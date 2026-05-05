using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class WeaponsService : MonoBehaviour, ITickable
{
    private TickService TickService;
    private GameState GameState;
    private ExpeditionState ExpeditionState;
    private CombatService CombatService;

    public void Initialize(GameState gameState, TickService Tick, CombatService combatService)
    {
        GameState = gameState;

        ExpeditionState = GameState.ExpeditionState;

        TickService = Tick;

        TickService.Subscribe(this);

        CombatService = combatService;
    }

    public void OnTick(float dt)
    {
        foreach (var weapon in ExpeditionState.Ship.Weapons)
        {
            if (weapon == null || weapon.Ammo == null)
                continue;

            UpdateWeapon(weapon, dt);
        }
    }

    void UpdateWeapon(WeaponInstance weapon, float dt)
    {
        ValidateTarget(weapon);

        AcquireTarget(weapon);

        HandleCooldown(weapon, dt);

        if (CanShoot(weapon))
        {
            ShootTarget(weapon);
        }
    }

    void ValidateTarget(WeaponInstance weapon)
    {
        if (weapon.CurrentTarget == null)
            return;

        if (weapon.CurrentTarget.State == EnemyHelper.EnemyState.Dead || weapon.CurrentTarget.State == EnemyHelper.EnemyState.Dying)
        {
            weapon.CurrentTarget = null;
            return;
        }

        if (weapon.CurrentTarget.Distance > weapon.ActualRange)
        {
            weapon.CurrentTarget = null;
        }
    }

    void AcquireTarget(WeaponInstance weapon)
    {
        var enemies = ExpeditionState.ActiveEnemies;

        if (enemies == null || enemies.Count == 0)
            return;

        List<EnemyInstance> valid = new();

        foreach (var enemy in enemies)
        {
            if (enemy.State == EnemyHelper.EnemyState.Dead || enemy.State == EnemyHelper.EnemyState.Dying)
                continue;

            if (enemy.Distance > weapon.ActualRange)
                continue;

            valid.Add(enemy);
        }

        if (valid.Count == 0)
            return;

        weapon.CurrentTarget = SelectTarget(valid, weapon.TargetType);
    }

    EnemyInstance SelectTarget(List<EnemyInstance> enemies, WeaponHelper.WeaponTarget type)
    {
        EnemyInstance best = enemies[0];

        switch (type)
        {
            case WeaponHelper.WeaponTarget.Closest:
                foreach (var e in enemies)
                    if (e.Distance < best.Distance)
                        best = e;
                break;

            case WeaponHelper.WeaponTarget.Farest:
                foreach (var e in enemies)
                    if (e.Distance > best.Distance)
                        best = e;
                break;

            case WeaponHelper.WeaponTarget.HighestHp:
                foreach (var e in enemies)
                    if (e.CurrentLife > best.CurrentLife)
                        best = e;
                break;

            case WeaponHelper.WeaponTarget.LowestHp:
                foreach (var e in enemies)
                    if (e.CurrentLife < best.CurrentLife)
                        best = e;
                break;

            default:
                break;
        }

        return best;
    }

    void HandleCooldown(WeaponInstance room, float dt)
    {
        if (room.Cooldown > 0)
            room.Cooldown -= dt;
    }

    bool CanShoot(WeaponInstance room)
    {
        if (room.CurrentTarget != null)
        {
            if (room.Cooldown <= 0)
            {
                if (room.CurrentTarget.CurrentLife > 0)
                {
                    return true;
                }
            }
        }

        if (GameState.ExpeditionState.ActiveEnemies.Count <= 0)
            return false;

        return false;
    }

    void ShootTarget(WeaponInstance weapon)
    {
        var target = weapon.CurrentTarget;

        if (target == null)
            return;

        if (target.State == EnemyHelper.EnemyState.Dead || target.State == EnemyHelper.EnemyState.Dying)
            return;

        if (target.CurrentLife <= 0)
            return;

        ExpeditionEvents.OnShoot?.Invoke(weapon, target);
        ShipDamage(weapon, target);

        weapon.Cooldown = 1 / weapon.ActualAttackSpeed;
    }

    private void ShipDamage(WeaponInstance weapon, EnemyInstance target)
    {
        double damage = weapon.ActualDamage + weapon.Ammo.Damage;

        if (target.MarkedEnemy)
        {
            damage *= 2;
        }

        target.CurrentLife -= damage;

        if (target.CurrentLife <= 0)
        {
            target.State = EnemyHelper.EnemyState.Dying;
        }
    }
}
