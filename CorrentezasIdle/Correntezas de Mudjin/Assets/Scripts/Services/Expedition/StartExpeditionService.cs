using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class StartExpeditionService : MonoBehaviour
{
    private GameState GameState;

    public void Initialize(GameState gameState)
    {
        GameState = gameState;

        LoadExpedition(GameState);
        LoadCurrency(GameState);
        LoadDestination(GameState);
        LoadShip(GameState);
    }

    void LoadExpedition(GameState Game)
    {
        Game.ExpeditionState = new ExpeditionState();
        Game.ExpeditionState.ActiveEnemies.Clear();
    }

    void LoadCurrency(GameState Game)
    {
        Game.ExpeditionState.ExpeditionCurrency = new Dictionary<CurrencyHelper.CurrencyType, CurrencyInstance>();

        if (Game.ExpeditionState.ExpeditionCurrency != null)
        {
            Game.ExpeditionState.ExpeditionCurrency.Clear();
        }

        foreach (var currency in Game.CompanyCurrency)
        {
            var original = currency.Value;

            var clone = new CurrencyInstance(original)
            {
                Amount = original.Amount
            };

            Game.ExpeditionState.ExpeditionCurrency.Add(currency.Key, clone);
        }
    }

    void LoadDestination(GameState Game)
    {
        if (!Game.FirstExpedition)
        {
            Game.ExpeditionState.CurrentDestination = Game.CurrentBase;
        }
    }

    void LoadShip(GameState Game)
    {
        Game.ShipState.Ship.MaxLife = Game.ShipState.Ship.BaseLife;
        Game.ShipState.Ship.CurrentLife = Game.ShipState.Ship.MaxLife;
    }
}
