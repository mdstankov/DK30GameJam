using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerHudController : MonoBehaviour
{
    // Start is called before the first frame update
	[SerializeField] GameObject InGameMenu = null;
	[SerializeField] GameObject MapNode = null;
	[SerializeField] GameObject IntroductionNode = null;
	[SerializeField] GameObject GameWonNode = null;
	[SerializeField] GameObject GameLostNode = null;


	[SerializeField] Text StoryProgress = null;
	[SerializeField] Text Timer = null;
	[SerializeField] Text Hint = null;

	private GameState m_GameState;
	private int m_TimeInSec = -1;
	
	void Awake( )
	{
		m_GameState = (GameState)FindObjectOfType(typeof(GameState));
		Assert.IsNotNull( m_GameState , "GameState is null" );
			   
		InGameMenu.SetActive( false );

		IntroductionNode.SetActive( false );
		GameWonNode.SetActive( false );
		GameLostNode.SetActive( false );

		MapNode.SetActive( false );
		UpdateTimer( );
		HideHint( );
	}

    void Start( )
    {
      
    }

    // Update is called once per frame
    void Update()
    {
		UpdateTimer( );
    }

	void UpdateTimer( )
	{
		int current_time = Mathf.RoundToInt( m_GameState.GetLevelTime( ) );

        if( m_TimeInSec != current_time )
		{
			m_TimeInSec = current_time;

			int min = m_TimeInSec/60;
			int seconds = m_TimeInSec % 60;

			string new_timer_text = min.ToString( "00" ) + ":" +  seconds.ToString( "00" );
			if( Timer )
				Timer.text = new_timer_text;
		}
	}

	public void ShowHint( )
	{		
		Hint.gameObject.SetActive( true );		
	}

	public void HideHint( )
	{	
		Hint.gameObject.SetActive( false );
	}

	public void ShowMap( )
	{		
		MapNode.SetActive( true );		
	}

	public void HideMap( )
	{	
		MapNode.SetActive( false );
	}

	public bool GetMapActive( )
	{
		return MapNode.activeSelf;	
	}
	
	public bool GeIngameMenuActive( )
	{
		return InGameMenu.activeSelf;	
	}

	public void SetIngameMenu( bool active )
	{
		InGameMenu.SetActive( active );
	}

	public void OnResumeGameButton( )
	{
		PlayerController player = (PlayerController)FindObjectOfType(typeof(PlayerController));
		if( player )
		{
			InGameMenu.SetActive( false );
			player.OnResumepGame( );
		}
	}
	
	public void OnReturnToLobby( )
	{
		LevelLoader level = (LevelLoader)FindObjectOfType(typeof(LevelLoader));
		if( level )
		{
			level.LoadLevel( Levels.Lobby );
		}

	}

	public void SetWinScreen( bool active )
	{
		GameWonNode.SetActive( active );
	}

	public void SetLoseScreen( bool active )
	{
		GameLostNode.SetActive( active );
	}

	public void SetIntroScreen( bool active )
	{
		IntroductionNode.SetActive( active );
	}

	public bool GetIntroActive( )
	{
		return IntroductionNode.activeSelf;
	}
	//

	public void OnReturnToLobbyButton( )
	{
		LevelLoader loader =  (LevelLoader)FindObjectOfType(typeof(LevelLoader));
		if( loader )
		{
			loader.LoadLevel( Levels.Lobby );
		}
		return;
	}

	public void OnIntroductionStartButton( )
	{
		PlayerController player = (PlayerController)FindObjectOfType(typeof(PlayerController));
		if( player )
		{
			IntroductionNode.SetActive( false );
			player.OnResumepGame( );
		}
		
		return;
	}

	public void UpdateDialogProgress( int unlocked , int total )
	{
		if( StoryProgress )
		{
			StoryProgress.text = unlocked.ToString( "00" ) + "/" + total.ToString( "00" );
		}
	}
}
