using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InteractableObject : MonoBehaviour
{
    [SerializeField] Dialog_State m_StartingState = null;
	[SerializeField] string	m_ObjectName = "";
	Text m_TextComponent;

	[SerializeField] Color FocusedNameColor = new Color( );
	[SerializeField] Color NormalNameColor = new Color( );

	[SerializeField] List<StoryFlag> m_ObjectFlags = new List<StoryFlag>( );

	bool isFocused = false;

	[SerializeField] GameObject m_Rotator = null;

    void Start( )
    {
        m_TextComponent = GetComponentInChildren<Text>( );
		if( m_TextComponent )
		{
			m_TextComponent.text = m_ObjectName;
			m_TextComponent.color = NormalNameColor;
		}
		else
		{
			Debug.Log( "TEXT NOT FOUND" );

		}
    }

    // Update is called once per frame
    void Update( )
    {
        if( isFocused && m_Rotator)
		{			
			Vector3 targetDir = Camera.main.transform.position - transform.position;
			targetDir.y = 0;
			//targetDir.z = 0;
			float step = 5.0f * Time.deltaTime;
			Vector3 newDir = Vector3.RotateTowards( m_Rotator.transform.forward * -1, targetDir , step, 0.0f );			
			
			m_Rotator.transform.rotation = Quaternion.LookRotation( newDir * -1 );			
		}
    }

    public bool OnUse( )
    {
        GameState state = (GameState)FindObjectOfType(typeof(GameState));

        if( state )      
        {
			DialogManager manager = state.GetDialogManager( );
			if( manager )
			{
				manager.StartConversation( m_StartingState , this );
				return true;

			}
			Debug.Log( "Dialog manager not found" );			
        }

		Debug.Log( "GameState not found" );
		return false;
    }

	public void OnFocusGained( )
	{	
		isFocused = true;
		m_TextComponent.color = FocusedNameColor;
	}

	public void OnFocusLost( )
	{
		isFocused = false;
		m_TextComponent.color = NormalNameColor;
	}
	
	public string GetName( )
	{
		return m_ObjectName;
	}
		
	public List<StoryFlag> GetObjectStoryFlags( )
	{
		return m_ObjectFlags;
	}

	public void AddFlag( StoryFlag flag )
	{
		m_ObjectFlags.Add( flag );
	}
	public bool HasFlag( StoryFlag flag )
	{
		foreach( StoryFlag object_flag in m_ObjectFlags )
		{
			if( object_flag == flag )
				return true;
		}

		return false;
	}

	public void SetOneFlag( StoryFlag flag )
	{
		m_ObjectFlags.Clear( );
		m_ObjectFlags.Add( flag );
	}

}
