using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatService : MonoBehaviour, ITickable
{
    private GameDatabase DataBase;
    private TickService TickService;
    private ShipState ShipState;
    private ExpeditionState Expedition;

    public void Initialize(ExpeditionState expeditionState, ShipState shipState, TickService Tick)
    {
        Expedition = expeditionState;

        ShipState = shipState;

        TickService = Tick;

        TickService.Subscribe(this);

        Debug.Log("EnemyControlSystem On");
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
        var enemies = Expedition.ActiveEnemies;

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
                ShipState.Ship.CurrentLife -= enemy.Model.Damage;

                enemy.Cooldown = 1.0 / enemy.Model.AttackSpeed;
                enemy.State = EnemyHelper.EnemyState.Cooldown;

                Debug.Log($"{enemy.Model.Name} atacou: {enemy.Model.Damage}");
            }
        }
    }

    public void ShipDamage(WeaponRoomInstance room, EnemyInstance target, double damage)
    {
        target.CurrentLife -= damage;

        Debug.Log($"{room.Weapon.Name} causou {damage} em {target.Model.Name}");
    }
}
