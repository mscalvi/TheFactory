using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Ship")]
public class ShipModel : ScriptableObject
{
    public string Id;
    public string Name;
    public string Description;

    public double Life;
    public double Speed;
    public double Armor;
    public int Size;
    public int UnnamedTripulation;
    public int NamedTripulation;
    public int WeaponRooms;
    public int OtherRooms;
}
