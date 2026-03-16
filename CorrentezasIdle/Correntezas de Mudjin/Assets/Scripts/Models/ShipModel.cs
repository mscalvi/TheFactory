using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Ship")]
public class ShipModel : ScriptableObject
{
    public string Id;
    public string Name;
    public string Description;

    public double LifeTotal;
    public double LifeRegen;
    public double Damage;
    public double AtackSpeed;
    public double Range;
}
