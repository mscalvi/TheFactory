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

    public double StartLife;
    public double BaseLife;
    public double ActualLife;

    public double StartSpeed;
    public double BaseSpeed;
    public double ActualSpeed;

    public double StartArmor;
    public double BaseArmor;
    public double ActualArmor;

    public double StartResistence;
    public double BaseResistence;
    public double ActualResistence;

    public double StartRepairPerTripulation;
    public double BaseRepairPerTripulation;
    public double ActualRepairPerTripulation;

    // Actual Value
    public double CurrentLife;

    public int WeaponSlots;
    public List<WeaponInstance> Weapons;

    public string UnlockId;
    public UnlockHelper.UnlockStatus UnlockStatus;

    public ShipInstance(ShipModel model)
    {
        Model = model;

        Id = model.Id;
        NameEN = model.NameEN;
        DescriptionEN = model.DescriptionEN;
        NamePT = model.NamePT;
        DescriptionPT = model.DescriptionPT;

        Size = model.Size;
        //MaxTripulation = model.Tripulation;

        StartLife = model.Life;
        BaseLife = model.Life;
        ActualLife = model.Life;
        CurrentLife = model.Life;

        StartSpeed = model.Speed;
        BaseSpeed = model.Speed;
        ActualSpeed = model.Speed;

        StartResistence = model.Resistence;
        BaseResistence = model.Resistence;
        ActualResistence = model.Resistence;

        StartRepairPerTripulation = 0;
        BaseRepairPerTripulation = 0;
        ActualRepairPerTripulation = 0;

        StartArmor = model.Armor;
        BaseArmor = model.Armor;
        ActualArmor = model.Armor;

        WeaponSlots = model.WeaponSlots;
        Weapons = new List<WeaponInstance>();

        UnlockId = model.UnlockId;
        UnlockStatus = model.UnlockStatus;
    }

    public ShipInstance()
    {

    }
}