using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingInstance
{
    public BuildingModel Model;

    public string Id;

    public string Name;

    public List<UpgradeInstance> Upgrades;

    public UpgradeHelper.UpgradeBuilding Type;

    public UnlockHelper.UnlockStatus UnlockStatus;

    public BuildingInstance(BuildingModel model)
    {
        Id = model.Id;

        Name = model.Name;

        Upgrades = model.Upgrades;

        Type = model.Type;

        UnlockStatus = model.UnlockStatus;
    }
}
