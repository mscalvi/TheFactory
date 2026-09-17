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

        if (!ingredients.TryGetValue(type, out var value))
            return 0;

        return value.Amount;
    }

    public void AddIngredient(AlchemyHelper.IngredientType type, double amount)
    {
        var dataIngredients = GameState.DataState.ingredients;

        if (!dataIngredients.ContainsKey(type))
            return;

        var ingredient = dataIngredients[type];

        if (ingredient.UnlockStatus != UnlockHelper.UnlockStatus.Unlocked &&
            ingredient.UnlockStatus != UnlockHelper.UnlockStatus.Available)
            return;

        ingredient.Amount = Get(type) + amount;

        ExpeditionEvents.IngredientIncome?.Invoke(ingredient, amount);
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

        if (product.UnlockStatus != UnlockHelper.UnlockStatus.Available &&
            product.UnlockStatus != UnlockHelper.UnlockStatus.Unlocked)
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

    // =========================================================
    // LOOT DE INGREDIENTES
    // =========================================================

    private void EnemyDeathReward(EnemyRuntime enemy)
    {
        if (!enemy.MarkedEnemy)
            return;

        for (int i = 0; i < GameState.ExpeditionState.ActualMaxMarkedLoot; i++)
        {
            RollIngredientDrop(
                GameHelper.ItemRarity.Common,
                enemy.CommonIngredient
            );

            RollIngredientDrop(
                GameHelper.ItemRarity.Uncommon,
                enemy.UncommonIngredient
            );

            RollIngredientDrop(
                GameHelper.ItemRarity.Rare,
                enemy.RareIngredient
            );

            RollIngredientDrop(
                GameHelper.ItemRarity.Legendary,
                enemy.LegendaryIngredient
            );
        }
    }

    private void RollIngredientDrop(
        GameHelper.ItemRarity rarity,
        AlchemyHelper.IngredientType ingredient)
    {
        // O inimigo não possui ingrediente dessa raridade.
        if (ingredient == AlchemyHelper.IngredientType.None)
            return;

        // O ingrediente ainda não está desbloqueado.
        if (!GameState.DataState.ingredients.TryGetValue(
                ingredient,
                out var instance))
            return;

        if (instance.UnlockStatus != UnlockHelper.UnlockStatus.Unlocked)
            return;

        // Verifica a chance da raridade.
        if (!RollRarityChance(rarity))
            return;

        // Deu certo!
        AddIngredient(ingredient, 1);
    }

    private bool RollRarityChance(GameHelper.ItemRarity rarity)
    {
        var weights =
            GameState.ExpeditionState.ActualIngredientRarityWeights;

        if (!weights.TryGetValue(rarity, out float chance))
            return false;

        // Se a chance estiver armazenada como porcentagem:
        // 50 = 50%
        // chance /= 100f;

        chance = Mathf.Clamp01(chance);

        float roll = UnityEngine.Random.value;

        Debug.Log(
            $"Loot {rarity}: " +
            $"Chance = {chance * 100f:F1}% | " +
            $"Roll = {roll * 100f:F1}%"
        );

        return roll <= chance;
    }

    // =========================================================
    // CHANCE EXTRA DE LOOT
    // =========================================================

    private bool RollChance(int time)
    {
        float roll = UnityEngine.Random.value;

        float chance =
            (float)(
                GameState.ExpeditionState.ActualNextLootChance *
                Mathf.Pow(
                    (float)GameState.ExpeditionState.ActualNextLootDecay,
                    time
                )
            );

        if (roll <= chance)
            return true;

        return false;
    }

    // =========================================================
    // EVENTS
    // =========================================================

    private void OnEnable()
    {
        ExpeditionEvents.OnMarkedEnemyDeath += EnemyDeathReward;
    }

    private void OnDisable()
    {
        ExpeditionEvents.OnMarkedEnemyDeath -= EnemyDeathReward;
    }
}