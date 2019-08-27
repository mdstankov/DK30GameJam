using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class CarScript : MonoBehaviour
{
	
	bool m_isDay9Car = false;	
	[SerializeField] public CarLocations LocationType;
	[SerializeField] public CarColors ColorType;

    [SerializeField] public Transform ToSpawnOfficerNode;

	[SerializeField] public Material MaterialPurple;
	[SerializeField] public Material MaterialBlue;
	[SerializeField] public Material MaterialYellow;
	   
	[SerializeField] Renderer ChildRenderer = null;

	void Awake( )
	{
		 SetRandomColor( );
	}

    void Start( )
	{
       
    }

	public bool GetIsCorrectCar( )
	{
		return m_isDay9Car;
	}

	public void SetIsCorrectCar( )
	{
		m_isDay9Car  = true;

		InteractableObject interaction = GetComponent<InteractableObject>( );
		interaction.AddFlag( StoryFlag.car_correct );
		Debug.Log( "DayCar is " + gameObject.name + " " + ColorType.ToString( ) + " " + LocationType.ToString( ) );
	}

    public Transform PoliceOfficerSpawnTransform( )
    {
		return ToSpawnOfficerNode;
    }

    public void SetRandomColor( )
    {
        //TODO:
		int random = Random.Range(0, 3);

		if( random == 0 ) // Yellow
		{
			ColorType = CarColors.Yellow;
			ChildRenderer.material = MaterialYellow;
		}
		else if( random == 1 ) // Blue
		{
			ColorType = CarColors.Blue;
			ChildRenderer.material = MaterialBlue;
		}
		else // Purple
		{
			ColorType = CarColors.Purple;
			ChildRenderer.material = MaterialPurple;
		}	

    }

}
