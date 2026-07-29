using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingDefinition : MonoBehaviour
{
    public BuildingInstance Building;
    private BuildingUi Ui;
    public Image BuildingIcon;

    public bool Note;

    public void Setup(BuildingInstance building, BuildingUi ui, GameState GameState)
    {
        Building = building;

        Ui = ui;

        Sprite icon = Resources.Load<Sprite>($"Sprites/Buildings/{building.Id}");

        if (icon != null)
            BuildingIcon.sprite = icon;
    }

    public void OnClick()
    {
        Ui.ShowUpgrades(Building);
    }
}
