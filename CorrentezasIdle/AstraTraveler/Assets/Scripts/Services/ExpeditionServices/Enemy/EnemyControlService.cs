using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using static GameHelper;

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
        EnemyContact(dt);
        EnemyMove(dt);
        EnemyDamage(dt);
    }

    void EnemyMove(float dt)
    {
        var enemies = Expedition.ActiveEnemies;

        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            var enemy = enemies[i];

            // Chegou na Área de Ataque ao Navio
            if (enemy.State == EnemyHelper.EnemyState.Moving || enemy.State == EnemyHelper.EnemyState.Dying)
            {
                if (enemy.Distance - enemy.Range > enemy.Speed * dt)
                {
                    enemy.Distance -= enemy.Speed * dt;
                } else
                {
                    enemy.Distance = enemy.Range;
                }

                if (enemy.Distance <= enemy.Range)
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
                    GameState.ExpeditionState.DamageTaken = true;
                    ExpeditionEvents.OnShipAtributeChange?.Invoke();
                }

                enemy.Cooldown = 1.0 / enemy.AttackSpeed;

                enemy.State = EnemyHelper.EnemyState.Cooldown;
            }
        }
    }

    void CheckEnemyLife(ProjectileInstance projectile, EnemyInstance enemy)
    {
        var enemies = Expedition.ActiveEnemies;

        if (enemy.State == EnemyHelper.EnemyState.Dead)
            return;

        if (enemy.State == EnemyHelper.EnemyState.Dying || enemy.ActualLife <= 0)
        {
            enemy.State = EnemyHelper.EnemyState.Dead;
            ExpeditionEvents.OnEnemyDeath?.Invoke(enemy);
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