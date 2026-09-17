using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LabDefinition : MonoBehaviour
{
    public LabInstance Building;
    private AlchemyPopUp Ui;
    public Image LabIcon;

    public bool Note;

    public void Setup(LabInstance building, AlchemyPopUp ui, GameState GameState)
    {
        Building = building;

        Ui = ui;

        Sprite icon = Resources.Load<Sprite>($"Sprites/Labs/{building.Id}");

        if (icon != null)
            LabIcon.sprite = icon;
    }

    public void OnClick()
    {
        Ui.ShowProducts(Building);
    }
}
