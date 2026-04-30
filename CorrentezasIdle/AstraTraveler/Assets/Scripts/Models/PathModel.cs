using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Path")]
public class PathModel : ScriptableObject
{
    public string Id;
    public string Name;
    public string Description;

    public double Distance;

    public DestinationModel Destination1;
    public DestinationModel Destination2;

    public DestinationHelper.DestinationRegion DestinationRegion1;
    public DestinationHelper.DestinationRegion DestinationRegion2;

    public PathHelper.PathType PathType;

    public UnlockHelper.UnlockStatus UnlockStatus;
}
