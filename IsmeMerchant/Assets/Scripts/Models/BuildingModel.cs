using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BuildingModel
{
    public string Id;

    public string NamePT;
    public string NameEN;
    public string DescriptionPT;
    public string DescriptionEN;

    public UpgradeHelper.UpgradeBuilding Type;

    public int Level;

    public string UnlockId;
    public UnlockHelper.UnlockStatus UnlockStatus;
}