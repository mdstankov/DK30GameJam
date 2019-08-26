using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityStandardAssets.Characters.FirstPerson;


public class DialogManager : MonoBehaviour
{
   // Dialog_State[] currentConversationStates;
	[SerializeField] Text DialogNameUI = null;
	[SerializeField] Text DialogTextUI = null;
	[SerializeField] Button[] DialogButtonsUI = null;	
	
    private Dialog_State m_CurrentState;	 
	private GameState m_GameState;   	
	
	private InteractableObject m_CurrentInteraction = null;

    void Start( )
    {
		gameObject.SetActive( false );
		m_GameState = (GameState)FindObjectOfType(typeof(GameState)); //TODO: Double Dependancy
    }
	
    public void StartConversation( Dialog_State state , InteractableObject intercation_object )
    {
		Debug.Log( "Start Conversation called" );
		gameObject.SetActive( true );
		
		m_CurrentInteraction = intercation_object;	

        if( state == null )
		{
			Debug.LogError( "Started conversation with null DialogState from " + m_CurrentInteraction.GetName( ) );
			return;
		}

        m_CurrentState = state;		
		///////
		DialogNameUI.text = m_CurrentInteraction.GetName( );
		SetupState( m_CurrentState );
		
    }

    public void EndConversation( )
	{
		if( m_CurrentInteraction )
			m_CurrentInteraction = null;

		if( m_CurrentState )
			m_CurrentState = null;

		Cursor.lockState = CursorLockMode.Confined;
		PlayerController player = (PlayerController)FindObjectOfType(typeof(PlayerController));
		player.OnResumepGame( );

		gameObject.SetActive( false );		
	}

	void SetupState( Dialog_State state )
	{		
		m_CurrentState = state;

		string state_text = m_GameState.FormatTagString( m_CurrentState.GetStateText( ) ); 
		DialogTextUI.text = state_text;
		//TODO:SetFlags
		
		foreach( StoryFlag flag in m_CurrentState.StoryFlags )
		{
			m_GameState.SetStoryFlag( flag );

		}

		foreach( StoryFlag flag in m_CurrentState.ObjectFlags )
		{
			m_CurrentInteraction.AddFlag( flag );
		}


		///////////////////////////////////////
        DialogResponceStruct[] stateResponces = m_CurrentState.GetAllResponces( );
		int btn_id = 0;

        foreach( DialogResponceStruct responce in stateResponces )  
        {          
			Button btn = DialogButtonsUI[btn_id];
			Assert.IsNotNull( btn , "UI BUTTON IS NULL" );
			btn.onClick.RemoveAllListeners( );
			btn.gameObject.SetActive( true );

			Text text_component = btn.GetComponent<Text>( );
            if( CanResponceBeShown( responce ) == false )
			{			
				if( state.hideMissingRequirements )
				{
					continue;
				}
				else
				{
					text_component.text = ( 1 + btn_id ).ToString( ) + ". <Missing Requirements>";
				}				
			}
			else
			{ 	
				string responce_string = ( 1 + btn_id ).ToString( ) + ". " + m_GameState.FormatTagString( responce.ResponceText );

				if( responce.TimeCost > 0 )				
				{
					responce_string = responce_string + " (-" +(responce.TimeCost ).ToString( ) + " sec.)";
				}
				
				text_component.text = responce_string;				
				btn.onClick.AddListener( ( ) => { OnResponceButtonClicked( responce ); } );
			}
			btn_id++;
        }

		//Disable the rest of the buttons
		for( ; btn_id < DialogButtonsUI.Length ; btn_id++ )
		{
			DialogButtonsUI[btn_id].gameObject.SetActive( false );
		}		
		
		if( m_CurrentState.Special != SpecialEvent.None )
		{
			m_GameState.OnSpecialEvent( m_CurrentState.Special );
		}
	}		   
	void OnResponceButtonClicked( DialogResponceStruct responce )
	{				
		if( responce.TimeCost > 0 )				
			m_GameState.ChangeTime( -responce.TimeCost );

		if( responce.EndDialog == true || responce.NextState == null )
		{
			EndConversation( );
			return;
		}

		Debug.Log( responce.NextState.name );
		SetupState( responce.NextState );
	}

    private bool CanResponceBeShown( DialogResponceStruct responce )
	{
		foreach( FlagResponceStruct responce_struct in responce.RequiresStory )
		{		
			bool has_flag = m_GameState.HasStoryFlag(responce_struct.Flag );
			bool expect_true = !responce_struct.HasToBeFalse;

			if( ( has_flag == true && expect_true == false ) || ( has_flag == false && expect_true == true ) )
			{
				//Debug.Log( "Missing story flag: " + responce_struct.Flag.ToString( ) );
				return false;
			}
		}

        foreach (FlagResponceStruct responce_struct in responce.RequiresObject)
		{
			bool has_flag =  m_CurrentInteraction.HasFlag(responce_struct.Flag);
			bool expect_true = !responce_struct.HasToBeFalse;

            if( ( has_flag == true && expect_true == false ) || ( has_flag == false && expect_true == true ) )
            {
				//Debug.Log( "Missing object flag: " + responce_struct.Flag.ToString( ) );
				return false;
			}
		}

		return true;
	}    
}
