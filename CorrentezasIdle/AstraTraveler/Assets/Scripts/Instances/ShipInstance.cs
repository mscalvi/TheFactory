using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static ShipModel;

public class ShipInstance
{
    public ShipModel Model;

    public string Id;
    public string NameEN;
    public string DescriptionEN;
    public string NamePT;
    public string DescriptionPT;

    public int Size;
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
    public double ActualLife;
    public double ActualSpeed;
    public double ActualArmor;
    public double ActualResistence;
    public double ActualRepairPerTripulation;

    public int WeaponSlots;
    public List<WeaponInstance> Weapons;

    public string UnlockId;
    public UnlockHelper.UnlockStatus UnlockStatus;

    public ShipInstance(ShipModel model)
    {
        Id = model.Id;
        NameEN = model.NameEN;
        DescriptionEN = model.DescriptionEN;
        NamePT = model.NamePT;
        DescriptionPT = model.DescriptionPT;

        Size = model.Size;
        MaxTripulation = model.Tripulation;

        StartLife = model.Life;
        BaseLife = model.Life;
        MaxLife = model.Life;
        ActualLife = model.Life;

        StartSpeed = model.Speed;
        BaseSpeed = model.Speed;
        MaxSpeed = model.Speed;
        ActualSpeed = model.Speed;

        StartResistence = model.Resistence;
        BaseResistence = model.Resistence;
        MaxResistence = model.Resistence;
        ActualResistence = model.Resistence;

        StartRepairPerTripulation = 0;
        BaseRepairPerTripulation = 0;
        MaxRepairPerTripulation = 0;
        ActualRepairPerTripulation = 0;

        StartArmor = model.Armor;
        BaseArmor = model.Armor;
        MaxArmor = model.Armor;
        ActualArmor = model.Armor;

        WeaponSlots = model.WeaponSlots;
        Weapons = new List<WeaponInstance>();

        UnlockId = model.UnlockId;
        UnlockStatus = model.UnlockStatus;
    }
}