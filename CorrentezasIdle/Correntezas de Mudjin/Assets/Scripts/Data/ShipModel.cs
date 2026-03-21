using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Ship")]
public class ShipModel : ScriptableObject
{
    public string Id;
    public string Name;
    public string Description;

    public double Life;
    public double Speed;
    public double Armor;
    public int Size;
    public int UnnamedTripulation;
    public int NamedTripulation;

    public List<WeaponRoomSlot> WeaponRooms;

    [System.Serializable]
    public class WeaponRoomSlot
    {
        public WeaponRoomModel RoomModel;
    }

    public List<OtherRoomSlot> OtherRooms;

    [System.Serializable]
    public class OtherRoomSlot
    {
        public OtherRoomModel RoomModel;
    }

    // Na Instance
    public UnlockHelper.UnlockStatus UnlockStatus;
}
