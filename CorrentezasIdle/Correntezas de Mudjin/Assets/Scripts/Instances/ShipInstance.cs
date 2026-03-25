using Unity.VisualScripting;
using UnityEngine;

public class ShipInstance
{
    public ShipModel Model;

    public double CurrentLife;
    public double MaxLife;
    public double RepairPerTripulation;

    public int TotalTripulation;

    public ShipInstance(ShipModel model)
    {
        Model = model;

        MaxLife = model.Life;
        CurrentLife = model.Life;

        RepairPerTripulation = 0;

        TotalTripulation = model.UnnamedTripulation + model.NamedTripulation;
    }
}