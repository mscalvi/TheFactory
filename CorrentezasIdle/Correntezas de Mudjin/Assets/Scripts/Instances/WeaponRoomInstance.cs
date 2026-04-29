using Unity.VisualScripting;
using UnityEngine;

public class WeaponRoomInstance
{
    public WeaponRoomModel Model;

    public string Id;
    public string SubId;

    public string Name;
    public string Description;

    public double RangeFactor;
    public double Angle;
    public double AngleMin;
    public double AngleMax;

    public RoomHelper.WeaponRoomType Type;

    public UnlockHelper.UnlockStatus UnlockStatus;

    public TripulationInstance Tripulation;
    public WeaponInstance Weapon;
    public AmmoInstance Ammo;

    public RoomHelper.RoomTarget TargetType;
    public EnemyInstance CurrentTarget;
    public double Cooldown;

    public WeaponRoomInstance(WeaponRoomModel model)
    {
        Id = model.Id;
        Name = model.Name;
        Description = model.Description;

        RangeFactor = model.RangeFactor;
        Angle = model.Angle;
        AngleMin = model.AngleMin;
        AngleMax = model.AngleMax;

        Type = model.Type;
        UnlockStatus = model.UnlockStatus;

        Tripulation = null;
        Weapon = null;
        Ammo = null;

        TargetType = RoomHelper.RoomTarget.Closest;
        CurrentTarget = null;
        Cooldown = 0;
    }

    public WeaponRoomInstance(WeaponRoomModel model, string subId)
    {
        Id = model.Id;
        SubId = subId;
        Name = model.Name;
        Description = model.Description;

        RangeFactor = model.RangeFactor;
        Angle = model.Angle;
        AngleMin = model.AngleMin;
        AngleMax = model.AngleMax;

        Type = model.Type;
        UnlockStatus = model.UnlockStatus;

        Tripulation = null;
        Weapon = null;
        Ammo = null;

        TargetType = RoomHelper.RoomTarget.Closest;
        CurrentTarget = null;
        Cooldown = 0;
    }

    public void Setup()
    {
        if (Weapon != null)
        {
            Cooldown = 1 / Weapon.ActualAttackSpeed;
        }
    }
}