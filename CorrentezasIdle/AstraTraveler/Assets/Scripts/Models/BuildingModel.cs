using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Game/Buildings")]
public class BuildingModel : ScriptableObject
{
    public string Id;

    public string Name;

    public UpgradeHelper.UpgradeBuilding Type;

    public List<UpgradeInstance> Upgrades;

    public UnlockHelper.UnlockStatus UnlockStatus;
}