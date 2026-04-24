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

    public double Get(IngredientHelper.IngredientType type)
    {
        var ingredients = GameState.CompanyState.CompanyIngredients;

        var ingredient = ingredients.TryGetValue(type, out var value) ? value.Amount : 0;

        var ingredientAmount = ingredients[type].Amount;

        return ingredientAmount;
    }

    public void AddIngredient(IngredientHelper.IngredientType type, double amount)
    {
        var dataIngredients = GameState.DataState.ingredients;

        foreach (var ingred in dataIngredients)
        {
            if (ingred.Value.Type == type)
            {
                if (ingred.Value.UnlockStatus != UnlockHelper.UnlockStatus.Unlocked)
                    return;
            }
        }

        var ingredients = GameState.CompanyState.CompanyIngredients;

        ingredients[type].Amount = Get(type) + amount;

        Debug.Log($"Adicionando {type} > {amount}. Novo total: {ingredients[type].Amount}");
    }

    public bool Spend(IngredientHelper.IngredientType type, double amount)
    {
        var ingredients = GameState.CompanyState.CompanyIngredients;
        double current = Get(type);

        if (current < amount)
            return false;

        ingredients[type].Amount = current - amount;

        return true;
    }

    IngredientHelper.IngredientType RollIngredient(EnemyInstance enemy)
    {
        var rarity = RollRarity();

        switch (rarity)
        {
            case IngredientHelper.IngredientRarity.Common:
                return enemy.CommonIngredient;
            case IngredientHelper.IngredientRarity.Uncommon:
                return enemy.UncommonIngredient;
            case IngredientHelper.IngredientRarity.Rare:
                return enemy.RareIngredient;
            case IngredientHelper.IngredientRarity.Legendary:
                return enemy.LegendaryIngredient;
        }

        return enemy.CommonIngredient;
    }

    private IngredientHelper.IngredientRarity RollRarity()
    {
        var w = GameState.ExpeditionState.IngredientRarityBaseWeights;

        float common = w[IngredientHelper.IngredientRarity.Common];
        float uncommon = w[IngredientHelper.IngredientRarity.Uncommon];
        float rare = w[IngredientHelper.IngredientRarity.Rare];
        float legendary = w[IngredientHelper.IngredientRarity.Legendary];

        float total = common + uncommon + rare + legendary;
        float roll = UnityEngine.Random.value * total;

        if (roll < common)
            return IngredientHelper.IngredientRarity.Common;

        roll -= common;

        if (roll < uncommon)
            return IngredientHelper.IngredientRarity.Uncommon;

        roll -= uncommon;

        if (roll < rare)
            return IngredientHelper.IngredientRarity.Rare;

        return IngredientHelper.IngredientRarity.Legendary;
    }

    private bool RollChance(int time)
    {
        float roll = UnityEngine.Random.value;

        float chance = (float)(GameState.ExpeditionState.NextLootChance * Mathf.Pow((float)GameState.ExpeditionState.NextLootDecay, time));

        if (roll < chance)
        {
            return true;
        }

        return false;
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

    void EnemyDeathReward(EnemyInstance enemy)
    {
        if (enemy.MarkedEnemy)
        {
            bool Luck = true;

            for (int i = 0; i < GameState.ExpeditionState.MaxMarkedLoot; i++)
            {
                if (i > 1)
                {
                    Luck = RollChance(i);
                }

                if (Luck)
                {
                    var ingredient = RollIngredient(enemy);
                    AddIngredient(ingredient, 1);
                }
            }
        }
    }

}