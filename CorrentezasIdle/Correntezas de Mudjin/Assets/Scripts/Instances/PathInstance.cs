using Unity.VisualScripting;
using UnityEngine;

public class PathInstance
{
    public PathModel Model;

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

    public PathInstance(PathModel model)
    {
        Id = model.Id;
        Name = model.Name;
        Description = model.Description;

        Distance = model.Distance;

        Destination1 = model.Destination1;
        Destination2 = model.Destination2;

        DestinationRegion1 = model.DestinationRegion1;
        DestinationRegion2 = model.DestinationRegion2;

        PathType = model.PathType;

        UnlockStatus = model.UnlockStatus;
    }
}