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
    public int ActualTripulation;
    public int MaxTripulation;

    // Model Base
    public double StartLife;
    public double StartSpeed;
    public double StartArmor;
    public double StartResistence;
    public double StartRepairPerTripulation;

    // Permanent Increase
    public double BaseLife;
    public double BaseSpeed;
    public double BaseArmor;
    public double BaseResistence;
    public double BaseRepairPerTripulation;

    // Expedition Increase
    public double MaxLife;
    public double MaxSpeed;
    public double MaxArmor;
    public double MaxResistence;
    public double MaxRepairPerTripulation;

    // Actual Value
    public double CurrentLife;
    public double CurrentSpeed;
    public double CurrentArmor;
    public double CurrentResistence;
    public double CurrentRepairPerTripulation;

    public List<WeaponSlot> WeaponSlots;
    public List<WeaponInstance> Weapons;

    public UnlockHelper.UnlockStatus UnlockStatus;

    public ShipInstance(ShipModel model)
    {
        Id = model.Id;
        Name = model.Name;
        Description = model.Description;

        Size = model.Size;
        MaxTripulation = model.Tripulation;
        ActualTripulation = 0;

        StartLife = model.Life;
        BaseLife = model.Life;
        MaxLife = model.Life;
        CurrentLife = model.Life;

        StartSpeed = model.Speed;
        BaseSpeed = model.Speed;
        MaxSpeed = model.Speed;
        CurrentSpeed = model.Speed;

        StartResistence = model.Resistence;
        BaseResistence = model.Resistence;
        MaxResistence = model.Resistence;
        CurrentResistence = model.Resistence;

        StartRepairPerTripulation = 0;
        BaseRepairPerTripulation = 0;
        MaxRepairPerTripulation = 0;
        CurrentRepairPerTripulation = 0;

        StartArmor = model.Armor;
        BaseArmor = model.Armor;
        MaxArmor = model.Armor;
        CurrentArmor = model.Armor;

        UnlockStatus = model.UnlockStatus;
    }
}