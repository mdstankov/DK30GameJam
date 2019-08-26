using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct FlagResponceStruct
{
    [SerializeField] public StoryFlag Flag;
    [SerializeField] public bool HasToBeFalse;
}

[System.Serializable]
public struct DialogResponceStruct
{
    [TextArea(2,10)] [SerializeField] public string ResponceText;
    [SerializeField] public Dialog_State NextState;
    [SerializeField] public FlagResponceStruct[] RequiresStory;
	[SerializeField] public FlagResponceStruct[] RequiresObject;
    
    [SerializeField] public bool EndDialog;
    [SerializeField] public float TimeCost;    
}

[CreateAssetMenu(menuName = "Dialog State")]
public class Dialog_State : ScriptableObject
{
    [TextArea(7,5)] [SerializeField] string stateText = "";
    [SerializeField] AudioClip Sound;
    [SerializeField] StoryFlag[] TriggersFlags;
    [SerializeField] public SpecialEvent Special;

    [SerializeField] DialogResponceStruct[] responces = null; 
    [SerializeField] public bool hideMissingRequirements = false;

    public string GetStateText( )
    {
        return stateText;
    }
	
    public DialogResponceStruct[] GetAllResponces( )
    {
        return responces;
    }

}
