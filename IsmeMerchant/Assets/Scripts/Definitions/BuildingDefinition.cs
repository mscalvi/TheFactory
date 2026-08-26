using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingDefinition : MonoBehaviour
{
    public BuildingInstance Building;
    private BuildingsPopUp Ui;
    private AcquisitionsPopUp UiShop;
    public Image BuildingIcon;

    public bool Note;

    public void SetupBuilding(BuildingInstance building, BuildingsPopUp ui, GameState GameState)
    {
        Building = building;

        Ui = ui;

        Sprite icon = Resources.Load<Sprite>($"Sprites/Buildings/{building.Id}");

        if (icon != null)
            BuildingIcon.sprite = icon;
    }

    public void SetupShop(BuildingInstance building, AcquisitionsPopUp ui, GameState GameState)
    {
        Building = building;

        UiShop = ui;

        Sprite icon = Resources.Load<Sprite>($"Sprites/Buildings/{building.Id}");

        if (icon != null)
            BuildingIcon.sprite = icon;
    }

    public void OnClick()
    {
        if (Ui != null)
        {
            Ui.ShowUpgrades(Building);
        }

        if (UiShop != null)
        {
            UiShop.ShowItems(Building.Type);
        }
    }
}
