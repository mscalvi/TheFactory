using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class WeaponsService : MonoBehaviour, ITickable
{
    private TickService TickService;
    private GameState GameState;
    private ExpeditionState ExpeditionState;

    double maxDamage = 0;
    public void Initialize(GameState gameState, TickService Tick)
    {
        GameState = gameState;

        ExpeditionState = GameState.ExpeditionState;

        TickService = Tick;

        TickService.Subscribe(this);
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
                    if (e.ActualLife > best.ActualLife)
                        best = e;
                break;

            case WeaponHelper.WeaponTarget.LowestHp:
                foreach (var e in enemies)
                    if (e.ActualLife < best.ActualLife)
                        best = e;
                break;

            default:
                break;
        }

        return best;
    }

    void HandleCooldown(WeaponInstance weapon, float dt)
    {
        if (weapon.Cooldown > 0)
            weapon.Cooldown -= dt;
    }

    bool CanShoot(WeaponInstance weapon)
    {
        if (GameState.ExpeditionState.ActiveEnemies.Count <= 0)
            return false;

        if (weapon.Ammo.CurrentAmmount <= 0)
        {
            return false;
        }

        if (weapon.CurrentTarget != null)
        {
            if (weapon.Cooldown <= 0)
            {
                if (weapon.CurrentTarget.ActualLife > 0)
                {
                    return true;
                }
            }
        }

        return false;
    }

    void ShootTarget(WeaponInstance weapon)
    {
        var target = weapon.CurrentTarget;

        if (target == null)
            return;

        if (target.State == EnemyHelper.EnemyState.Dead || target.State == EnemyHelper.EnemyState.Dying)
            return;

        if (target.ActualLife <= 0)
            return;

        ExpeditionEvents.OnShoot?.Invoke(weapon, target);

        ShipDamage(weapon, target);

        weapon.Ammo.CurrentAmmount--;
        Debug.Log(weapon.Ammo.CurrentAmmount);

        if (weapon.Ammo.CurrentAmmount <= 0)
        {
            weapon.Ammo.IsReloading = true;
        }

        weapon.Cooldown = 1 / weapon.ActualAttackSpeed;
    }

    private void ShipDamage(WeaponInstance weapon, EnemyInstance target)
    {
        double damage = weapon.ActualDamage + weapon.Ammo.ActualDamage;

        if (target.MarkedEnemy)
        {
            damage *= 2;
        }

        target.ActualLife -= damage;

        if (target.ActualLife <= 0)
        {
            target.State = EnemyHelper.EnemyState.Dying;
        }

        if(maxDamage < damage)
        {
            maxDamage = damage;
        }
    }
}
