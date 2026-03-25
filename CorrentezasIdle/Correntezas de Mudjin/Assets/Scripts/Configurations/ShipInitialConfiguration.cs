using System.Collections.Generic;

[System.Serializable]
public class ShipInitialConfiguration
{
    public ShipModel Ship;
    public List<WeaponRoomInitialConfiguration> WeaponRooms;
    // public List<OtherRoomInitialConfiguration> OtherRooms;
}