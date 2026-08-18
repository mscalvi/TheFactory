using System;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using static GameHelper;
using static UnityEngine.GraphicsBuffer;

public class EnemyControllerService : MonoBehaviour, ITickable
{
    private TickService TickService;
    private GameState GameState;
    private ExpeditionState Expedition;

    public void Initialize(GameState game, TickService Tick)
    {
        GameState = game;
        Expedition = GameState.ExpeditionState;

        TickService = Tick;

        TickService.Subscribe(this);
    }

    void OnDestroy()
    {
        TickService?.Unsubscribe(this);
    }

    public void OnTick(float dt)
    {
        EnemyRegen(dt);
        EnemyContact(dt);
        EnemyMove(dt);
        EnemyDamage(dt);
    }

    void EnemyRegen(float dt)
    {
        var enemies = Expedition.ActiveEnemies;

        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            var enemy = enemies[i];

            if (enemy.ActualLife <= 0 || enemy.ActualLife >= enemy.StartLife)
                continue;

            enemy.RegenTimer += dt;

            if (enemy.RegenTimer >= 1f)
            {
                enemy.RegenTimer = 0f;

                enemy.ActualLife += enemy.LifeRegen;

                if (enemy.ActualLife > enemy.StartLife)
                {
                    enemy.ActualLife = enemy.StartLife;
                }
            }
        }
    }

    void EnemyMove(float dt)
    {
        var enemies = Expedition.ActiveEnemies;

        double shipRadius = Expedition.Ship.Size / 2.0;

        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            var enemy = enemies[i];

            if (enemy.State == EnemyHelper.EnemyState.Moving || enemy.State == EnemyHelper.EnemyState.Dying)
            {
                double enemyRadius = enemy.Size / 2.0;
                double stopDistance = enemy.Range + shipRadius + enemyRadius;

                if (enemy.Distance - stopDistance > enemy.Speed * dt)
                {
                    enemy.Distance -= enemy.Speed * dt;
                }
                else
                {
                    enemy.Distance = stopDistance;
                }

                if (enemy.Distance <= stopDistance)
                {
                    if (enemy.State != EnemyHelper.EnemyState.Dying)
                    {
                        enemy.State = EnemyHelper.EnemyState.Arrival;
                    }
                }
            }
        }
    }

    void EnemyContact(float dt)
    {
        var enemies = Expedition.ActiveEnemies;

        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            var enemy = enemies[i];

            // Chegou na Área de Ataque ao Navio
            if (enemy.State == EnemyHelper.EnemyState.Arrival)
            {
                enemy.Cooldown = 0.1;
                enemy.State = EnemyHelper.EnemyState.Damaging;
            }
        }
    }

    void EnemyDamage(float dt)
    {
        var enemies = GameState.ExpeditionState.ActiveEnemies;

        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            var enemy = enemies[i];

            enemy.Cooldown -= dt;

            if (enemy.State == EnemyHelper.EnemyState.Cooldown && enemy.Cooldown <= 0)
            {
                enemy.State = EnemyHelper.EnemyState.Damaging;
            }

            if (enemy.State == EnemyHelper.EnemyState.Damaging)
            {
                double AbsoluteArmor = GameState.ExpeditionState.Ship.ActualArmor;
                double RelativeArmor = GameState.ExpeditionState.Ship.ActualResistence;

                double RealDamage = (enemy.Damage - (enemy.Damage * (RelativeArmor / 100))) - AbsoluteArmor;

                if (RealDamage > 0)
                {
                    GameState.ExpeditionState.Ship.CurrentLife -= RealDamage;
                    if (GameState.ExpeditionState.Ship.CurrentLife < 0)
                    {
                        GameState.ExpeditionState.Ship.CurrentLife = 0;
                    }
                    GameState.ExpeditionState.DamageTaken = true;
                    ExpeditionEvents.OnShipAtributeChange?.Invoke();
                }

                enemy.Cooldown = 1.0 / enemy.AttackSpeed;

                enemy.State = EnemyHelper.EnemyState.Cooldown;
            }
        }
    }

    void CheckEnemyLife(ProjectileRuntime projectile, EnemyRuntime enemy, Vector3 position)
    {
        var enemies = Expedition.ActiveEnemies;

        if (enemy.State == EnemyHelper.EnemyState.Dead)
            return;

        if (enemy.State == EnemyHelper.EnemyState.Dying || enemy.ActualLife <= 0)
        {
            enemy.State = EnemyHelper.EnemyState.Dead;
            ExpeditionEvents.OnEnemyDeath?.Invoke(enemy, position);
            enemies.Remove(enemy);
        }
    }

    void OnEnable()
    {
        ExpeditionEvents.OnProjectileHit += CheckEnemyLife;
    }

    void OnDisable()
    {
        ExpeditionEvents.OnProjectileHit -= CheckEnemyLife;
    }
}