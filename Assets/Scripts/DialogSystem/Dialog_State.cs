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
    [SerializeField] public float TimeCost;   
	[SerializeField] public Dialog_State ReplaceStartState;
    [SerializeField] public bool EndDialog;     
}

[CreateAssetMenu(menuName = "Dialog State")]
public class Dialog_State : ScriptableObject
{
    [TextArea(7,5)] [SerializeField] string stateText = "";
    [SerializeField] AudioClip Sound = null;
    [SerializeField] public StoryFlag[] StoryFlags;
	[SerializeField] public StoryFlag[] ObjectFlags;
    [SerializeField] public SpecialEvent Special = SpecialEvent.None;

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
