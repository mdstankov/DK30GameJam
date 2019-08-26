using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StoryFlag
{
    None = 0,
    car_color = 1 << 0,
    car_location = 1 << 1,
    police_at_car = 1 << 2,
    police_called = 1 << 3,
    cop_car_clear = 1 << 4,
    car_inspected = 1 << 5,
    car_correct = 1 << 6,
}

public enum SpecialEvent
{
    Spawn_Cop,
    Game_Won,
    Game_Lost,

}

public enum Levels
{
    Lobby,
    Introduction,
    Game,
    WinScreen,
    LoseScreen,
}


public enum CarLocations
{
	Unknown,
	LocalStore,
	Club,
	MLG_Arena,
}

public enum CarColors
{
	Unknown,
	Red,
	Green,
	Yellow,
	White,
}

public class GameState : MonoBehaviour
{
	float LevelTimer = 9.0f * 60.0f;
	bool isTimerPaused = true;
		 
	//GameOver State
	public GameObject DialogPrefab;
	DialogManager m_DialogManager = null;
	
	StoryFlag m_StoryFlags  = StoryFlag.None;

	private CarColors m_CarColor = CarColors.Unknown;
	private CarLocations m_CarLocation = CarLocations.Unknown;
    private CarScript m_CorrectCar = null;
    

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
				//ShowGameOver;
			}
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
            SetStoryFlag(StoryFlag.police_at_car);

            return;
        }
        else if (e == SpecialEvent.Game_Won)
        {
            //TODO:

        }
        else if (e == SpecialEvent.Game_Lost )
        {
            //TODO:

        }

        Debug.LogError("Special Event not handled" + e.ToString());
    }

    public string FormatTagString( string original )
	{

		original = original.Replace( "{car_color}" , GetCarColorString( ) );

		original = original.Replace( "{car_location}" , GetCarLocationString( ) );
		
		return original;
	}

	///
	string GetCarColorString( )
	{
		switch ( m_CarColor )
        {
			case CarColors.Green:
				return "green";

			case CarColors.Red:
				return "red";

			case CarColors.White:
				return "white";

			case CarColors.Yellow:
				return "yellow";

			default:
				return "unknown";
        }
	}

	string GetCarLocationString( )
	{
		switch ( m_CarLocation )
        {
			case CarLocations.Unknown:
				return "unknown";

			case CarLocations.Club:
				return "at the store";

			case CarLocations.MLG_Arena:
				return "MLG Arena";

			default:
				return "unknown";
        }		
	}
	   	  
	//--------------
	public void SetStoryFlag( StoryFlag flag )
	{
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
		m_CarLocation = CarLocations.LocalStore;
		
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
