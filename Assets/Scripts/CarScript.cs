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

     void Start( )
	{
        SetRandomColor( );
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
		Debug.Log( "DayCar is " + gameObject.name );
	}

    public void SpawnPoliceOfficerNextToCar( )
    {
        //TODO:
        

    }

    public void SetRandomColor( )
    {
        //TODO:


    }

}
