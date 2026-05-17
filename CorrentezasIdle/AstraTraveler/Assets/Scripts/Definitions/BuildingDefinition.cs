using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingDefinition : MonoBehaviour
{
    public BuildingInstance Building;

    public TMP_Text Name;
    private BuildingUi Ui;

    public bool Note;

    public void Setup(BuildingInstance building, BuildingUi ui, GameState GameState)
    {
        Building = building;

        Ui = ui;

        if (GameState.ActualLanguage == GameState.Language.English)
        {
            Name.text = building.NameEN;
        }

        if (GameState.ActualLanguage == GameState.Language.Portugues)
        {
            Name.text = building.NamePT;
        }

    }

    public void OnClick()
    {
        Ui.ShowUpgrades(Building);
    }
}
