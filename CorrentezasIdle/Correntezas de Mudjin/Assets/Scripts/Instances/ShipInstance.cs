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

    public int Size;
    public int UnnamedTripulation;
    public int NamedTripulation;

    // Model Base
    public double StartLife;
    public double StartSpeed;
    public double StartArmor;

    // Permanent Increase
    public double BaseLife;
    public double BaseSpeed;
    public double BaseArmor;

    // Expedition Increase
    public double MaxLife;
    public double MaxSpeed;
    public double MaxArmor;

    // Actual Value
    public double CurrentLife;
    public double CurrentSpeed;
    public double CurrentArmor;

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

        Size = model.Size;
        UnnamedTripulation = model.UnnamedTripulation;
        NamedTripulation = model.NamedTripulation;

        StartLife = model.Life;
        BaseLife = model.Life;
        MaxLife = model.Life;
        CurrentLife = model.Life;

        StartSpeed = model.Speed;
        BaseSpeed = model.Speed;
        MaxSpeed = model.Speed;
        CurrentSpeed = model.Speed;

        StartArmor = model.Armor;
        BaseArmor = model.Armor;
        MaxArmor = model.Armor;
        CurrentArmor = model.Armor;

        RepairPerTripulation = 0;

        TotalTripulation = model.UnnamedTripulation + model.NamedTripulation;

        WeaponRoomSlots = model.WeaponRoomSlots;
        OtherRoomSlots = model.OtherRoomSlots;

        WeaponsRooms = null;

        UnlockStatus = model.UnlockStatus;
    }
}