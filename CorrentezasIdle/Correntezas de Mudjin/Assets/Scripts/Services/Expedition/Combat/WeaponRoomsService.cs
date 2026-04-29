using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class WeaponRoomsService : MonoBehaviour, ITickable
{
    private TickService TickService;
    private GameState GameState;
    private ShipState ShipState;
    private ExpeditionState ExpeditionState;
    private CombatService CombatService;

    public void Initialize(GameState gameState, ExpeditionState expeditionState, ShipState shipState, TickService Tick, CombatService combatService)
    {
        GameState = gameState;

        ShipState = shipState;

        ExpeditionState = expeditionState;

        TickService = Tick;

        TickService.Subscribe(this);

        CombatService = combatService;
    }

    public void OnTick(float dt)
    {
        foreach (var room in ShipState.Ship.WeaponsRooms)
        {
            if (room == null || room.Weapon == null)
                continue;

            UpdateRoom(room, dt);
        }
    }

    void UpdateRoom(WeaponRoomInstance room, float dt)
    {
        if (room.Weapon == null)
            return;

        bool RoomInUse = false;

        foreach (var rooms in GameState.TripulationState.TripulationWeaponAssignment)
        {
            if (rooms.Value.Id == room.Id)
            {
                RoomInUse = true;
            }
        }

        if (!RoomInUse)
            return;

        ValidateTarget(room);

        AcquireTarget(room);

        HandleCooldown(room, dt);

        if (CanShoot(room))
        {
            ShootTarget(room);
        }
    }

    void ValidateTarget(WeaponRoomInstance room)
    {
        if (room.CurrentTarget == null)
            return;

        if (room.CurrentTarget.State == EnemyHelper.EnemyState.Dead)
        {
            room.CurrentTarget = null;
            return;
        }

        if (room.CurrentTarget.Distance > room.Weapon.ActualRange)
        {
            room.CurrentTarget = null;
        }
    }

    void AcquireTarget(WeaponRoomInstance room)
    {
        var enemies = ExpeditionState.ActiveEnemies;

        if (enemies == null || enemies.Count == 0)
            return;

        List<EnemyInstance> valid = new();

        foreach (var enemy in enemies)
        {
            if (enemy.State == EnemyHelper.EnemyState.Dead)
                continue;

            if (enemy.Distance > room.Weapon.ActualRange)
                continue;

            if (!IsAngleInRange(enemy.Angle, room.AngleMin, room.AngleMax))
                continue;

            valid.Add(enemy);
        }

        if (valid.Count == 0)
            return;

        room.CurrentTarget = SelectTarget(valid, room.TargetType);
    }

    EnemyInstance SelectTarget(List<EnemyInstance> enemies, RoomHelper.RoomTarget type)
    {
        EnemyInstance best = enemies[0];

        switch (type)
        {
            case RoomHelper.RoomTarget.Closest:
                foreach (var e in enemies)
                    if (e.Distance < best.Distance)
                        best = e;
                break;

            case RoomHelper.RoomTarget.Farest:
                foreach (var e in enemies)
                    if (e.Distance > best.Distance)
                        best = e;
                break;

            case RoomHelper.RoomTarget.HighestHp:
                foreach (var e in enemies)
                    if (e.CurrentLife > best.CurrentLife)
                        best = e;
                break;

            case RoomHelper.RoomTarget.LowestHp:
                foreach (var e in enemies)
                    if (e.CurrentLife < best.CurrentLife)
                        best = e;
                break;

            default:
                break;
        }

        return best;
    }

    void HandleCooldown(WeaponRoomInstance room, float dt)
    {
        if (room.Cooldown > 0)
            room.Cooldown -= dt;
    }

    bool CanShoot(WeaponRoomInstance room)
    {
        if (room.CurrentTarget != null)
        {
            if (room.Cooldown <= 0)
            {
                if (room.CurrentTarget.Life > 0)
                {
                    return true;
                }
            }
        }

        if (GameState.ExpeditionState.ActiveEnemies.Count <= 0)
            return false;

        return false;
    }

    void ShootTarget(WeaponRoomInstance room)
    {
        var target = room.CurrentTarget;

        if (target == null)
            return;

        if (target.Life < 0)
            return;

        double damage = room.Weapon.ActualDamage + room.Ammo.Damage;

        ExpeditionEvents.OnShoot?.Invoke(room, target);

        if (target.MarkedEnemy)
        {
            damage *= 2;
        }

        CombatService.ShipDamage(room, target, damage);

        room.Cooldown = 1 / room.Weapon.ActualAttackSpeed;
    }

    bool IsAngleInRange(double angle, double min, double max)
    {
        if (angle >= min && angle <= max)
            return true;
        else
            return false;
    }
}
