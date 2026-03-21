using Unity.VisualScripting;
using UnityEngine;

public class ShipInstance
{
    public ShipModel Model;

    public double CurrentLife;

    public ShipInstance(ShipModel model)
    {
        Model = model;
        CurrentLife = model.Life;
    }
}