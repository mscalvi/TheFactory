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
        var ingredients = GameState.DataState.ingredients;

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
                if (ingred.Value.UnlockStatus != UnlockHelper.UnlockStatus.Unlocked && ingred.Value.UnlockStatus != UnlockHelper.UnlockStatus.Available)
                    return;
            }
        }

        dataIngredients[type].Amount = Get(type) + amount;

        Debug.Log($"Adicionando {type} > {amount}. Novo total: {dataIngredients[type].Amount}");
    }

    public bool Spend(IngredientHelper.IngredientType type, double amount)
    {
        var ingredients = GameState.DataState.ingredients;
        double current = Get(type);

        if (current < amount)
            return false;

        ingredients[type].Amount = current - amount;

        return true;
    }

    private IngredientHelper.IngredientType RollIngredient(EnemyRuntime enemy)
    {
        GameHelper.ItemRarity rarity = RollRarity();

        while (true)
        {
            IngredientHelper.IngredientType ingredient = rarity switch
            {
                GameHelper.ItemRarity.Common => enemy.CommonIngredient,
                GameHelper.ItemRarity.Uncommon => enemy.UncommonIngredient,
                GameHelper.ItemRarity.Rare => enemy.RareIngredient,
                GameHelper.ItemRarity.Legendary => enemy.LegendaryIngredient,
                _ => enemy.CommonIngredient
            };

            if (ingredient != IngredientHelper.IngredientType.None)
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

        Debug.Log($"Chance de Ingrediente: {chance * 100}");

        if (roll <= chance)
        {
            Debug.Log("Sucesso!");
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