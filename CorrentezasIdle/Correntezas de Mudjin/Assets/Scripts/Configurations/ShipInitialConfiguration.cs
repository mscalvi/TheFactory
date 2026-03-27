using System.Collections.Generic;

[System.Serializable]
public class ShipInitialConfiguration
{
    public ShipInstance Ship;
    public List<WeaponRoomInitialConfiguration> WeaponRooms;
    // public List<OtherRoomInitialConfiguration> OtherRooms;
}