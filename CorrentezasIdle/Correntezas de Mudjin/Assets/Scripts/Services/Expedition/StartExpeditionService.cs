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
        LoadIngredients(GameState);
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

        foreach (var currency in Game.CompanyState.CompanyCurrency)
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
        if (GameState.ProgressState.m000)
        {
            Game.ExpeditionState.CurrentDestination = Game.CompanyState.CurrentBase;
        }
    }

    void LoadShip(GameState Game)
    {
        Game.ShipState.Ship.MaxLife = Game.ShipState.Ship.BaseLife;
        Game.ShipState.Ship.CurrentLife = Game.ShipState.Ship.MaxLife;
    }

    void LoadIngredients(GameState Game)
    {
        Game.ExpeditionState.RarityBaseWeights = new Dictionary<IngredientHelper.IngredientRarity, float>()
        {
            { IngredientHelper.IngredientRarity.Common, 100 },
            { IngredientHelper.IngredientRarity.Uncommon, 0 },
            { IngredientHelper.IngredientRarity.Rare, 0 },
            { IngredientHelper.IngredientRarity.Legendary, 0 }
        };

        // Conferir e Aplicar Upgrades na CompanyUpgrades
    }
}
