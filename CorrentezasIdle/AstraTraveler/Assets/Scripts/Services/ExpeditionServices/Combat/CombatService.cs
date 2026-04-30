using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatService : MonoBehaviour, ITickable
{
    private GameState GameState;
    private TickService TickService;
    private ExpeditionUi UiService;
    private ExpeditionState ExpeditionState;

    public void Initialize(GameState gameState, TickService Tick)
    {
        GameState = gameState;

        ExpeditionState = GameState.ExpeditionState;

        TickService = Tick;

        TickService.Subscribe(this);
    }

    void OnDestroy()
    {
        TickService?.Unsubscribe(this);
    }

    public void OnTick(float dt)
    {
        EnemyDamage(dt);
    }

    void EnemyDamage(float dt)
    {
        var enemies = ExpeditionState.ActiveEnemies;

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
                double AbsoluteArmor = ExpeditionState.Ship.CurrentArmor;
                double RelativeArmor = ExpeditionState.Ship.CurrentArmor;

                double RealDamage = (enemy.Damage - (enemy.Damage * RelativeArmor)) - AbsoluteArmor;

                ExpeditionState.Ship.CurrentLife -= RealDamage;

                ExpeditionState.DamageTaken = true;

                enemy.Cooldown = 1.0 / enemy.AttackSpeed;
                enemy.State = EnemyHelper.EnemyState.Cooldown;

                ExpeditionEvents.OnShipAtributeChange?.Invoke();
            }
        }
    }

    public void ShipDamage(EnemyInstance target, double damage)
    {
        target.CurrentLife -= damage;
    }
}
