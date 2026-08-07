using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyTypeDefinition : MonoBehaviour
{
    public EnemyHelper.EnemyType EnemyType;
    private BestiaryPopUp Ui;
    public Image EnemyTypeIcon;

    public bool Note;

    public void Setup(EnemyHelper.EnemyType enemy, BestiaryPopUp ui, GameState GameState)
    {
        EnemyType = enemy;

        Ui = ui;

        Sprite icon = Resources.Load<Sprite>($"Sprites/EnemiesTypes/{EnemyType}");

        if (icon != null)
            EnemyTypeIcon.sprite = icon;
    }

    public void OnClick()
    {
        Ui.ShowSpecies(EnemyType);
    }

}
