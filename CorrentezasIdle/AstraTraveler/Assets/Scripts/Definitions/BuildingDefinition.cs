using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingDefinition : MonoBehaviour
{
    public BuildingInstance building;

    public bool Note;

    public void Setup(BuildingInstance Building)
    {
        building = Building;

        Note = false;
    }
}
