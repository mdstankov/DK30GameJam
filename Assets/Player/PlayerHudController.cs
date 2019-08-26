using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class PlayerHudController : MonoBehaviour
{
    // Start is called before the first frame update

	[SerializeField] Image MapImage = null;
	[SerializeField] Text Timer = null;
	[SerializeField] Text Hint = null;

	private GameState m_GameState;
	private int m_TimeInSec = -1;
	
    void Start()
    {
        m_GameState = (GameState)FindObjectOfType(typeof(GameState));
		Assert.IsNotNull( m_GameState , "GameState is null" );

		MapImage.gameObject.SetActive( false );
		UpdateTimer( );
		HideHint( );
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
		MapImage.gameObject.SetActive( true );		
	}

	public void HideMap( )
	{	
		MapImage.gameObject.SetActive( false );
	}

	public bool GetMapActive( )
	{
		return MapImage.gameObject.activeSelf;	
	}

}
