using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public enum StoryFlag
{
    None = 0,
    car_color = 1 << 0,
    car_location = 1 << 1,
    police_at_car = 1 << 2,
    police_called = 1 << 3,
    cop_clear = 1 << 4,
    car_inspected = 1 << 5,
    car_correct = 1 << 6,
    jp_called = 1 << 7,
    felicity_called = 1 << 8,
    manfried_used = 1 << 9


}

public enum SpecialEvent
{
	None,
    Spawn_Cop,
	Cop_Clear,
    Game_Won,
    Game_Lost,

}

public enum CarLocations
{
	Unknown = 0,
	LocalStore = 1,
	Club = 2,
	MLG_Arena = 3,
}

public enum CarColors
{
	Unknown,
	Yellow,
	Purple,
	Blue,
}

public class GameState : MonoBehaviour
{
	[SerializeField] GameObject CopPrefab = null;

	[SerializeField] AudioClip GameWinSound = null;
	[SerializeField] AudioClip GameLoseSound = null;
	
	[SerializeField] float LevelTimer = 9.0f * 60.0f;
	bool isTimerPaused = true;
		 
	//GameOver State
	public GameObject DialogPrefab;
	DialogManager m_DialogManager = null;
	
	StoryFlag m_StoryFlags  = StoryFlag.None;

	private CarColors m_CarColor = CarColors.Unknown;
	private CarLocations m_CarLocation = CarLocations.Unknown;
    private CarScript m_CorrectCar = null;
	private GameObject CopObject = null;
    
    void Start( )
    {
		GameObject prefab = Instantiate( DialogPrefab, new Vector3( 0, 0, 0 ) , Quaternion.identity ) as GameObject;
        m_DialogManager = prefab.GetComponent<DialogManager>( );
		if( m_DialogManager == null )
		{
			Debug.LogError( "MISSING DIALOG MANAGER IN GAME STATE" );
		}

		GenerateRandomCarGoal( );
    }
    // Update is called once per frame
    void Update()
    {
        
		if( isTimerPaused == true )
		{
			LevelTimer = LevelTimer - Time.deltaTime;

			if( LevelTimer <= 0 )
			{
				isTimerPaused = true;
				OnGameOver( );
			}
		}
    }

	void OnGameOver( )
	{
		if( m_DialogManager )
		{
			m_DialogManager.EndConversation( );
		}

        PlayerController player = (PlayerController)FindObjectOfType(typeof(PlayerController));
		if( player )
		{
			if( GameLoseSound ) 
				{ player.PlaySound( GameLoseSound );}

			player.StartGameLost( );
		}
	}


	public void PauseTimer( )
	{
		isTimerPaused = false;
	}

	public void ResumeTimer( )
	{
		isTimerPaused = true;
	}

	public void ChangeTime( float time_to_add )
	{
		LevelTimer += time_to_add;
		if( LevelTimer <= 0 )
		{
			OnGameOver( );
		}
	}

	public float GetLevelTime( )
	{
		return LevelTimer;
	}
    
	public DialogManager GetDialogManager( )
	{
		return m_DialogManager;
	}
    
    public void OnSpecialEvent( SpecialEvent e )
    {
        if (e == SpecialEvent.Spawn_Cop)
        {
            //TODO;
			if( HasStoryFlag ( StoryFlag.police_at_car ) == false )
			{ 
				 SetStoryFlag(StoryFlag.police_at_car);
			}
           
			if( CopObject )
			{
				Debug.Log( "Cop already spawned" );
				return;
			}

			if( CopPrefab && m_CorrectCar )
			{

				Transform transform = m_CorrectCar.PoliceOfficerSpawnTransform( );
				CopObject = Instantiate( CopPrefab, transform.position , transform.rotation ) as GameObject;

			}

            return;
        }
		else if( e == SpecialEvent.Cop_Clear )
		{
			if( CopObject )
			{
				GameObject model = CopObject.transform.Find("Model").gameObject;
				if( model )
				{
					model.transform.Rotate( 90 , 0 , 0 );
				}
				
			}
			return;
		}
        else if (e == SpecialEvent.Game_Won)
        {
			if( m_DialogManager )
			{
				m_DialogManager.EndConversation( );
			}

            PlayerController player = (PlayerController)FindObjectOfType(typeof(PlayerController));
			if( player )
			{

				if( GameWinSound ) 
					{ player.PlaySound( GameWinSound );	 }
											 
				player.StartGameWon( );
			}
			return;
        }
        else if (e == SpecialEvent.Game_Lost )
        {
			if( m_DialogManager )
			{
				m_DialogManager.EndConversation( );
			}

            PlayerController player = (PlayerController)FindObjectOfType(typeof(PlayerController));
			if( player )
			{		
				if( GameLoseSound ) 
					{ player.PlaySound( GameLoseSound );}
					
				player.StartGameLost( );
			}
			return;
        }

        Debug.LogError("Special Event not handled" + e.ToString());
    }

    public string FormatTagString( string original )
	{

		original = original.Replace( "{car_color}" , GetCarColorString( ) );

		original = original.Replace( "{car_location}" , GetCarLocationString( ) );
		
		original = original.Replace( "{random_number}" , Random.Range( 1000 , 10000 ).ToString( ) );

		return original;
	}

	///
	string GetCarColorString( )
	{
		switch ( m_CarColor )
        {
			case CarColors.Yellow:
				return "YELLOW";

			case CarColors.Blue:
				return "BLUE";

			case CarColors.Purple:
				return "PURPLE";

			default:
				return "unknown";
        }
	}

	string GetCarLocationString( )
	{
		switch ( m_CarLocation )
        {
			case CarLocations.LocalStore:
				return "MOSTLY SHOPPING STORE";

			case CarLocations.Club:
				return "FUNDAY MONDAY CLUB";

			case CarLocations.MLG_Arena:
				return "MLG STADIUM ";

			default:
				return "unknown";
        }		
	}
	   	  
	//--------------
	public void SetStoryFlag( StoryFlag flag )
	{
		if( HasStoryFlag ( flag) ) { return; }

		Debug.Log( "Story flag set: " + flag.ToString( ) );
		m_StoryFlags = SetFlag( m_StoryFlags , flag );
	}

	//  public void UnsetStoryFlag( StoryFlag flag )
	//  {
	//Debug.Log( "Story flag un-set: " + flag.ToString( ) );
	//      m_StoryFlags = m_StoryFlags & (~flag);
	//  }

	public bool HasStoryFlag(StoryFlag flag)
	{
		return ( m_StoryFlags & flag ) == flag;
	}

	//  public void ToggleStoryFlag( StoryFlag flag )
	//  {
	//Debug.Log( "Story flag toggle: " + flag.ToString( ) );
	//      m_StoryFlags ^= flag;
	//  }


	public static StoryFlag SetFlag (StoryFlag a, StoryFlag b)
	{
		return a | b;
	}
 
	public static StoryFlag UnsetFlag (StoryFlag a, StoryFlag b)
	{
		return a & (~b);
	}
 
	// Works with "None" as well
	public static bool HasFlag (StoryFlag a, StoryFlag b)
	{
		return (a & b) == b;
	}
 
	public static StoryFlag ToogleFlag (StoryFlag a, StoryFlag b)
	{
		return a ^ b;
	}
	
/////////////////////////////////////////////////////////
///
	void GenerateRandomCarGoal( )
	{
		//Pick random car location
		m_CarLocation = (CarLocations)Random.Range( 1, 4 );
		Debug.Log( "Car Location is " + m_CarLocation.ToString( ) );
		//Find all object Tagged Car in the level

		List<GameObject> Cars = new List<GameObject>( );
		
		foreach( GameObject car in GameObject.FindGameObjectsWithTag("CAR"))
		{ 
			Cars.Add(car);
        }

		if( Cars.Count == 0 )
		{
			Debug.LogError( "Cars TAG not found" );
			return;
		}

		List<CarScript> CarScripts = new List<CarScript>( );
        
        //Find all cars in that location
        foreach (GameObject car in Cars )
		{
			CarScript car_script = car.GetComponent<CarScript>( );
			if( car_script )
			{
				if( car_script.LocationType == m_CarLocation )
				{
					CarScripts.Add(car_script);

				}
			}
		}
		
		if( CarScripts.Count == 0 )
		{
			Debug.LogError( "CarScripts in CARS not found correct location" );
			return;
		}
       
        // Get the random car, and get its color( randomly generated at start)
        m_CorrectCar = CarScripts [Random.Range(0, CarScripts.Count)];

		if(m_CorrectCar)
		{
			m_CarColor = m_CorrectCar.ColorType;
            m_CorrectCar.SetIsCorrectCar( );

			return;
        }

		Debug.Log( "SOMETHING IS WRONG WITH RANDOM CAR PICKING" );
	}














}
