using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingInstance
{
    public BuildingModel Model;

    public string Id;
    public string NamePT;
    public string NameEN;
    public string DescriptionPT;
    public string DescriptionEN;

    public bool Note;

    public UpgradeHelper.UpgradeBuilding Type;
    public UpgradeHelper.BuildingScope Scope;

    public int Level;

    public string UnlockId;
    public UnlockHelper.UnlockStatus UnlockStatus;

    public BuildingInstance(BuildingModel model)
    {
        Id = model.Id;

        NameEN = model.NameEN;
        NamePT = model.NamePT;
        DescriptionPT = model.DescriptionPT;
        DescriptionEN = model.DescriptionEN;

        Scope = model.Scope;

        Note = false;

        Type = model.Type;

        Level = model.Level;

        UnlockId = model.UnlockId;
        UnlockStatus = model.UnlockStatus;
    }

    public BuildingInstance()
    {

    }
}
