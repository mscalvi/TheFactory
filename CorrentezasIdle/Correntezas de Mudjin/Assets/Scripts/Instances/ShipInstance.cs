using Unity.VisualScripting;
using UnityEngine;

public class ShipInstance
{
    public ShipModel Model;

    public double CurrentLife;


    public double Damage;
    public double AttackSpeed;
    public double Range;
    public double Cooldown;

    public ShipInstance(ShipModel model)
    {
        Model = model;
        CurrentLife = model.Life;

        Damage = 5;
        AttackSpeed = 1;
        Range = 20;
        Cooldown = 1;
    }
}