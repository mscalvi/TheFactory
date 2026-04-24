using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingDefinition : MonoBehaviour
{
    public BuildingInstance building;

    public List<UpgradeInstance> Upgrades;

    public bool Note;

    public void Setup(BuildingInstance Building)
    {
        building = Building;

        Upgrades = new List<UpgradeInstance>();

        Note = false;

        foreach (var upgrade in building.Upgrades)
        {
            Upgrades.Add(upgrade);
        }
    }
}
