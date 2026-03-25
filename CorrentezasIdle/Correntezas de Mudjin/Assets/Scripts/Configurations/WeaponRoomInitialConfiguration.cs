using System.Collections.Generic;

[System.Serializable]
public class WeaponRoomInitialConfiguration
{
    public string RoomId;
    public TripulationModel Tripulation;
    public WeaponModel Weapon;
    public AmmoModel Ammo;
    public RoomHelper.RoomTarget TargetType;
}