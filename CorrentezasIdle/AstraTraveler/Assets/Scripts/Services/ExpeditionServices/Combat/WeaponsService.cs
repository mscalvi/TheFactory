using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class WeaponsService : MonoBehaviour, ITickable
{
    private TickService TickService;
    private GameState GameState;
    private ExpeditionState ExpeditionState;

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

    private void ValidateTarget(WeaponInstance weapon)
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
            return;
        }

        return;
    }

    void AcquireTarget(WeaponInstance weapon)
    {
        var enemies = ExpeditionState.ActiveEnemies;

        if (enemies == null || enemies.Count == 0)
            return;

        List<EnemyRuntime> valid = new();

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

    EnemyRuntime SelectTarget(List<EnemyRuntime> enemies, WeaponHelper.WeaponTarget type)
    {
        EnemyRuntime best = enemies[0];

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

        if (weapon.Ammo.IsReloading)
            return false;

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

        if (GameState.ExpeditionState.ActiveEnemies.Count <= 0)
            return;

        weapon.Ammo.CurrentAmmount--;

        ExpeditionEvents.OnShoot?.Invoke(weapon, target);

        ShipDamage(weapon, target);

        if (weapon.Ammo.CurrentAmmount <= 0)
        {
            weapon.Ammo.CurrentRecharge = weapon.Ammo.ActualRecharge;
            weapon.Ammo.IsReloading = true;
            ExpeditionEvents.OnRechargeStart?.Invoke(weapon.Ammo);
        }

        weapon.Cooldown = 1 / weapon.ActualAttackSpeed;
    }

    private void ShipDamage(WeaponInstance weapon, EnemyRuntime target)
    {
        double damage = weapon.ActualDamage + weapon.Ammo.ActualDamage;

        if (target.MarkedEnemy)
        {
            damage *= GameState.ExpeditionState.ActualClickDamage;
        }

        double criticalRoll = Random.Range(0, 100);
        double realRoll = criticalRoll / 100;

        if (realRoll < weapon.ActualPrecision)
        {
            damage *= weapon.ActualCriticalDamage;
            Debug.Log($"Crítico! Dano Total: {damage}");
        }

        target.ActualLife -= damage;

        if (target.ActualLife <= 0)
        {
            target.State = EnemyHelper.EnemyState.Dying;
        }

        //Debug.Log($"Tiro: {damage} de Dano Total\n Arma: {weapon.ActualDamage} + Munição: {weapon.Ammo.ActualDamage} Critico: {realRoll < weapon.ActualPrecision} + Marcado: {target.MarkedEnemy}");
    }
}
