using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static ShipModel;

public class ShipInstance
{
    public ShipModel Model;

    public string Id;
    public string Name;
    public string Description;

    public double Life;
    public double Speed;
    public double Armor;
    public int Size;
    public int UnnamedTripulation;
    public int NamedTripulation;

    public double CurrentLife;
    public double MaxLife;
    public double RepairPerTripulation;

    public int TotalTripulation;

    public List<WeaponRoomSlot> WeaponRoomSlots;
    public List<WeaponRoomInstance> WeaponsRooms;

    public List<OtherRoomSlot> OtherRoomSlots;
    //public List<OtherRoomInstance> OtherRooms;

    public UnlockHelper.UnlockStatus UnlockStatus;

    public ShipInstance(ShipModel model)
    {
        Id = model.Id;
        Name = model.Name;
        Description = model.Description;
        Life = model.Life;
        Speed = model.Speed;
        Armor = model.Armor;
        Size = model.Size;
        UnnamedTripulation = model.UnnamedTripulation;
        NamedTripulation = model.NamedTripulation;

        CurrentLife = model.Life;
        MaxLife = model.Life;
        RepairPerTripulation = 0;

        TotalTripulation = model.UnnamedTripulation + model.NamedTripulation;

        WeaponRoomSlots = model.WeaponRoomSlots;
        OtherRoomSlots = model.OtherRoomSlots;

        WeaponsRooms = null;

        UnlockStatus = model.UnlockStatus;
    }
}