using Unity.VisualScripting;
using UnityEngine;

public class WeaponRoomInstance
{
    public WeaponRoomModel Model;

    public WeaponModel Weapon;
    public TripulationModel Tripulation;
    public AmmoModel Ammo;

    public RoomHelper.RoomTarget TargetType;

    public double Cooldown;
    public EnemyInstance CurrentTarget;

    public WeaponRoomInstance(WeaponRoomModel model)
    {
        Model = model;
        CurrentTarget = null;
    }

    public void Setup()
    {
        if (Weapon != null)
        {
            Cooldown = 1 / Weapon.AttackSpeed;
        }
    }
}