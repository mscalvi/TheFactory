using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Destination")]
public class DestinationModel : ScriptableObject
{
    public string Id;
    public string Name;
    public string Description;

    public DestinationHelper.DestinationType DestinationType;
    public DestinationHelper.DestinationRegion DestinationRegion;

    public UnlockHelper.UnlockStatus UnlockStatus;
}
