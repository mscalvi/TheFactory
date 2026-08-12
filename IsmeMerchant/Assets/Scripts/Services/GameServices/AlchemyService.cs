using System;
using System.Collections.Generic;
using UnityEngine;

public class AlchemyService : MonoBehaviour
{
    private GameState GameState;
    private IngredientService IngredientService;
    private CurrencyService CurrencyService;

    public void Initialize(GameState game, IngredientService ingredients, CurrencyService currency)
    {
        GameState = game;
        IngredientService = ingredients;
        CurrencyService = currency;
    }

    private void Update()
    {
        if (GameState == null)
            return;

        UpdateProductions();
    }

    public void StartProduction(ProductInstance product)
    {
        if (product.NextProduction != default)
            return;

        product.NextProduction =
            DateTime.UtcNow.AddSeconds(product.ActualTime);
    }

    private void UpdateProductions()
    {
        DateTime now = DateTime.UtcNow;

        foreach (var product in GameState.DataState.products.Values)
        {
            if (product.UnlockStatus != UnlockHelper.UnlockStatus.Unlocked)
                continue;

            if (product.BuyCount <= 0)
                continue;

            if (product.NextProduction == default)
                continue;

            if (product.NextProduction > now)
                continue;

            ProcessProduction(product, now);
        }
    }

    private void ProcessProduction(ProductInstance product, DateTime now)
    {
        double productionTime = product.ActualTime;

        if (productionTime <= 0)
            return;

        double elapsedSeconds =
            (now - product.NextProduction).TotalSeconds;

        long cycles =
            (long)Math.Floor(elapsedSeconds / productionTime) + 1;

        double amount =
            product.IncomeAmmount *
            product.BuyCount *
            cycles;

        CurrencyService.Add(
            product.IncomeType,
            amount
        );

        product.NextProduction =
            product.NextProduction.AddSeconds(
                productionTime * cycles
            );
    }
}