using System;
using UnityEngine;

public class EnemyProgressService : MonoBehaviour
{
    private GameState GameState;
    private ExpeditionState Expedition;

    public void Initialize(GameState game)
    {
        GameState = game;
        Expedition = GameState.ExpeditionState;
    }

    public void ApplyProgression(EnemyRuntime enemy)
    {
        int days = Expedition.DayCounter - 1;

        enemy.StartLife = Calculate(enemy.StartLife, enemy.LifeGrowth, days);
        enemy.ActualLife = enemy.StartLife;

        enemy.LifeRegen = Calculate(enemy.LifeRegen, enemy.LifeRegenGrowth, days);
        enemy.Speed = Calculate(enemy.Speed, enemy.SpeedGrowth, days);
        enemy.Armor = Calculate(enemy.Armor, enemy.ArmorGrowth, days);
        enemy.Range = Calculate(enemy.Range, enemy.RangeGrowth, days);
        enemy.Damage = Calculate(enemy.Damage, enemy.DamageGrowth, days);
        enemy.AttackSpeed = Calculate(enemy.AttackSpeed, enemy.AttackSpeedGrowth, days);
        enemy.SpawnDistance = Calculate(enemy.SpawnDistance, enemy.SpawnDistanceGrowth, days);
    }

    private double Calculate(double baseValue, double growth, int time)
    {
        return baseValue + ((growth * baseValue) * time);
    }
}