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

        Debug.Log("ShipControlService On");
    }

    void LoadExpedition(GameState Game)
    {
        Game.ExpeditionState = new ExpeditionState();

        Debug.Log($"Expedition Ativa");
    }

    void LoadCurrency(GameState Game)
    {
        Game.ExpeditionState.ExpeditionCurrency = new Dictionary<CurrencyHelper.CurrencyType, CurrencyInstance>();

        Debug.Log($"Carregando Currencies");

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

            Debug.Log($"Currency carregada: {currency.Value.Id}");
        }

        Debug.Log("Dinheiro Carregado");
    }
}
