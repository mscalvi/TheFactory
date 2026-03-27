using System.Collections.Generic;

[System.Serializable]
public class WeaponRoomInitialConfiguration
{
    public string RoomId;
    public TripulationInstance Tripulation;
    public WeaponInstance Weapon;
    public AmmoInstance Ammo;
    public RoomHelper.RoomTarget TargetType;
}