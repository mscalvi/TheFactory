using Unity.VisualScripting;
using UnityEngine;

public class DestinationInstance
{
    public DestinationModel Model;

    public string Id;
    public string Name;
    public string Description;

    public DestinationHelper.DestinationType DestinationType;
    public DestinationHelper.DestinationRegion DestinationRegion;

    public UnlockHelper.UnlockStatus UnlockStatus;

    public DestinationInstance(DestinationModel model)
    {
        Id = model.Id;
        Name = model.Name;
        Description = model.Description;

        DestinationType = model.DestinationType;
        DestinationRegion = model.DestinationRegion;

        UnlockStatus = model.UnlockStatus;
    }
}