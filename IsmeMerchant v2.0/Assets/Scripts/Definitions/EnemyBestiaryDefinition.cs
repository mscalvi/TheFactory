using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBestiaryDefinition : MonoBehaviour
{
    public EnemyInstance Enemy;
    private BestiaryPopUp Ui;
    public Image EnemyIcon;

    public bool Note;

    public void Setup(EnemyInstance enemy, BestiaryPopUp ui, GameState GameState)
    {
        Enemy = enemy;

        Ui = ui;

        Sprite icon = Resources.Load<Sprite>($"Sprites/Enemies/{enemy.Id}");

        if (icon != null)
            EnemyIcon.sprite = icon;
    }

    public void OnClick()
    {
        Ui.ShowInfo(Enemy);
    }
}
