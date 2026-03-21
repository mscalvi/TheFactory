using System.Collections.Generic;

[System.Serializable]
public class RunController
{
    public ShipModel Ship;
    public List<RoomConfiguration> Rooms;
}

[System.Serializable]
public class RoomConfiguration
{
    public string RoomId;
    public TripulationModel Tripulation;
    public WeaponModel Weapon;
    public AmmoModel Ammo;
    public RoomHelper.RoomTarget TargetType;
}