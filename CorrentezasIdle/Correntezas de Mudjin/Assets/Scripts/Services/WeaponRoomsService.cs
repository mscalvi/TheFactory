using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class WeaponRoomsService : MonoBehaviour, ITickable
{
    private TickService TickService;
    private ShipState ShipState;
    private ExpeditionState ExpeditionState;
    private CombatService CombatService;

    public void Initialize(ExpeditionState expeditionState, ShipState shipState, TickService Tick, CombatService combatService)
    {
        ShipState = shipState;
        ExpeditionState = expeditionState;

        TickService = Tick;

        TickService.Subscribe(this);

        CombatService = combatService;

        Debug.Log("WeaponRoomService On");
    }

    public void OnTick(float dt)
    {
        foreach (var room in ShipState.WeaponRooms)
        {
            if (room == null || room.Weapon == null)
                continue;

            UpdateRoom(room, dt);
        }
    }

    void UpdateRoom(WeaponRoomInstance room, float dt)
    {
        ValidateTarget(room);

        if (room.CurrentTarget == null)
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

        if (room.CurrentTarget.Distance > room.Weapon.Range)
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

            if (enemy.Distance > room.Weapon.Range)
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
        return room.CurrentTarget != null &&
               room.Cooldown <= 0;
    }

    void ShootTarget(WeaponRoomInstance room)
    {
        var target = room.CurrentTarget;

        if (target == null)
            return;

        double damage = room.Weapon.Damage + room.Ammo.Damage;

        CombatService.ShipDamage(room, target, damage);

        room.Cooldown = 1 / room.Weapon.AttackSpeed;
    }
}
