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
    public int MaxUnnamedTripulation;
    public int MaxNamedTripulation;
    public int MaxTotalTripulation;
    public int ActualUnnamedTripulation;
    public int ActualNamedTripulation;
    public int ActualTotalTripulation;

    // Model Base
    public double StartLife;
    public double StartSpeed;
    public double StartArmor;
    public double StartRepairPerTripulation;

    // Permanent Increase
    public double BaseLife;
    public double BaseSpeed;
    public double BaseArmor;
    public double BaseRepairPerTripulation;

    // Expedition Increase
    public double MaxLife;
    public double MaxSpeed;
    public double MaxArmor;
    public double MaxRepairPerTripulation;

    // Actual Value
    public double CurrentLife;
    public double CurrentSpeed;
    public double CurrentArmor;
    public double CurrentRepairPerTripulation;

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
        MaxUnnamedTripulation = model.UnnamedTripulation;
        MaxNamedTripulation = model.NamedTripulation;
        MaxTotalTripulation = model.UnnamedTripulation + model.NamedTripulation;
        ActualUnnamedTripulation = 0;
        ActualNamedTripulation = 0;
        ActualTotalTripulation = 0;

        StartLife = model.Life;
        BaseLife = model.Life;
        MaxLife = model.Life;
        CurrentLife = (int)model.Life;

        StartSpeed = model.Speed;
        BaseSpeed = model.Speed;
        MaxSpeed = model.Speed;
        CurrentSpeed = model.Speed;

        StartRepairPerTripulation = 0;
        BaseRepairPerTripulation = 0;
        MaxRepairPerTripulation = 0;
        CurrentRepairPerTripulation = 0;

        StartArmor = model.Armor;
        BaseArmor = model.Armor;
        MaxArmor = model.Armor;
        CurrentArmor = model.Armor;

        WeaponRoomSlots = model.WeaponRoomSlots;
        OtherRoomSlots = model.OtherRoomSlots;

        WeaponsRooms = null;

        UnlockStatus = model.UnlockStatus;
    }
}