using System;
using System.Collections.Generic;
using UnityEngine;
using static CurrencyHelper;

public class IngredientService : MonoBehaviour
{
    private GameState GameState;

    public void Initialize(GameState game)
    {
        GameState = game;
    }

    public double Get(AlchemyHelper.IngredientType type)
    {
        var ingredients = GameState.DataState.ingredients;

        var ingredient = ingredients.TryGetValue(type, out var value) ? value.Amount : 0;

        var ingredientAmount = ingredients[type].Amount;

        return ingredientAmount;
    }

    public void AddIngredient(AlchemyHelper.IngredientType type, double amount)
    {
        var dataIngredients = GameState.DataState.ingredients;

        foreach (var ingred in dataIngredients)
        {
            if (ingred.Value.Type == type)
            {
                if (ingred.Value.UnlockStatus != UnlockHelper.UnlockStatus.Unlocked && ingred.Value.UnlockStatus != UnlockHelper.UnlockStatus.Available)
                    return;
            }
        }

        dataIngredients[type].Amount = Get(type) + amount;

        ExpeditionEvents.IngredientIncome?.Invoke(GameState.DataState.ingredients[type], amount);
    }

    public bool Spend(AlchemyHelper.IngredientType type, double amount)
    {
        var ingredients = GameState.DataState.ingredients;
        double current = Get(type);

        if (current < amount)
            return false;

        ingredients[type].Amount = current - amount;

        GameEvents.OnIngredientChange?.Invoke(type);

        return true;
    }

    public bool BuyProduct(ProductInstance product)
    {
        if (!CanBuyProduct(product))
            return false;

        if (!(product.UnlockStatus == UnlockHelper.UnlockStatus.Available || product.UnlockStatus == UnlockHelper.UnlockStatus.Unlocked))
            return false;

        List<AlchemyHelper.IngredientType> affectedIngredients =
            new(product.ActualCosts.Keys);

        foreach (var cost in product.ActualCosts)
        {
            Spend(cost.Key, cost.Value);
        }

        product.BuyCount++;
        UpdateProductCosts(product);

        product.UnlockStatus = UnlockHelper.UnlockStatus.Unlocked;
        product.CanBuy = false;

        foreach (var ingredient in affectedIngredients)
        {
            CanBuyIngredient(ingredient);
        }

        return true;
    }

    public void CanBuyIngredient(AlchemyHelper.IngredientType ingredient)
    {
        foreach (var product in GameState.DataState.products.Values)
        {
            if (!product.ActualCosts.ContainsKey(ingredient))
                continue;

            product.CanBuy = CanBuyProduct(product);
        }
    }

    public bool CanBuyProduct(ProductInstance product)
    {
        if (product.UnlockStatus != UnlockHelper.UnlockStatus.Available &&
            product.UnlockStatus != UnlockHelper.UnlockStatus.Unlocked)
            return false;

        foreach (var cost in product.ActualCosts)
        {
            if (Get(cost.Key) < cost.Value)
                return false;
        }

        return true;
    }

    private void UpdateProductCosts(ProductInstance product)
    {
        product.ActualCosts.Clear();

        foreach (var cost in product.BaseCosts)
        {
            double actualCost =
                cost.Value * Math.Pow(1.3, product.BuyCount);

            product.ActualCosts[cost.Key] =
                Math.Ceiling(actualCost);
        }
    }

    // Drop
    private AlchemyHelper.IngredientType RollIngredient(EnemyRuntime enemy)
    {
        GameHelper.ItemRarity rarity = RollRarity();

        while (true)
        {
            AlchemyHelper.IngredientType ingredient = rarity switch
            {
                GameHelper.ItemRarity.Common => enemy.CommonIngredient,
                GameHelper.ItemRarity.Uncommon => enemy.UncommonIngredient,
                GameHelper.ItemRarity.Rare => enemy.RareIngredient,
                GameHelper.ItemRarity.Legendary => enemy.LegendaryIngredient,
                _ => enemy.CommonIngredient
            };

            if (ingredient != AlchemyHelper.IngredientType.None)
            {
                var instance = GameState.DataState.ingredients[ingredient];

                if (instance.UnlockStatus == UnlockHelper.UnlockStatus.Unlocked)
                {
                    return ingredient;
                }
            }

            // downgrade
            switch (rarity)
            {
                case GameHelper.ItemRarity.Legendary:
                    rarity = GameHelper.ItemRarity.Rare;
                    break;

                case GameHelper.ItemRarity.Rare:
                    rarity = GameHelper.ItemRarity.Uncommon;
                    break;

                case GameHelper.ItemRarity.Uncommon:
                    rarity = GameHelper.ItemRarity.Common;
                    break;

                default:
                    return enemy.CommonIngredient;
            }
        }
    }

    private GameHelper.ItemRarity RollRarity()
    {
        var weight = GameState.ExpeditionState.ActualIngredientRarityWeights;

        float common = weight[GameHelper.ItemRarity.Common];
        float uncommon = weight[GameHelper.ItemRarity.Uncommon];
        float rare = weight[GameHelper.ItemRarity.Rare];
        float legendary = weight[GameHelper.ItemRarity.Legendary];

        float total = common + uncommon + rare + legendary;
        float roll = UnityEngine.Random.value * total;

        if (roll < common)
            return GameHelper.ItemRarity.Common;

        roll -= common;

        if (roll < uncommon)
            return GameHelper.ItemRarity.Uncommon;

        roll -= uncommon;

        if (roll < rare)
            return GameHelper.ItemRarity.Rare;

        return GameHelper.ItemRarity.Legendary;
    }

    private bool RollChance(int time)
    {
        float roll = UnityEngine.Random.value;

        float chance = (float)(GameState.ExpeditionState.ActualNextLootChance * Mathf.Pow((float)GameState.ExpeditionState.ActualNextLootDecay, time));

        if (roll <= chance)
        {
            return true;
        }

        return false;
    }

    void EnemyDeathReward(EnemyRuntime enemy)
    {
        if (enemy.MarkedEnemy)
        {
            bool Luck = true;

            for (int i = 0; i < GameState.ExpeditionState.ActualMaxMarkedLoot; i++)
            {
                Luck = RollChance(i);

                if (Luck)
                {
                    var ingredient = RollIngredient(enemy);
                    AddIngredient(ingredient, 1);
                }
            }
        }
    }

    // Event
    void OnEnable()
    {
        ExpeditionEvents.OnMarkedEnemyDeath += EnemyDeathReward;
    }

    void OnDisable()
    {
        ExpeditionEvents.OnMarkedEnemyDeath -= EnemyDeathReward;
    }

}