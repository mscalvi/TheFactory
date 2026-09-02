using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerDefinition : MonoBehaviour
{
    private GameInstance GameInstance;
    private PlayerInstance PlayerInstance;

    [SerializeField] TextMeshProUGUI PlayerName;
    [SerializeField] TextMeshProUGUI DeckName;
    [SerializeField] TextMeshProUGUI TotalLife;

    public void Setup(GameInstance Game, PlayerInstance Player)
    {
        GameInstance = Game;
        PlayerInstance = Player;

        SetLife();
        SetName();
    }

    private void SetLife()
    {
        TotalLife.text = PlayerInstance.CurrentLife.ToString();
    }
    private void SetName()
    {
        PlayerName.text = PlayerInstance.Name;
        DeckName.text = PlayerInstance.CurrentDeck.Name;
    }

    public void PlusButton()
    {
        PlayerInstance.CurrentLife++;
        SetLife();
    }
    public void MinusButton()
    {
        PlayerInstance.CurrentLife--;
        SetLife();
    }
    public void PlusHold()
    {
        PlayerInstance.CurrentLife += 10;
        SetLife();
    }
    public void MinusHold()
    {
        PlayerInstance.CurrentLife -= 10;
        SetLife();
    }
}
