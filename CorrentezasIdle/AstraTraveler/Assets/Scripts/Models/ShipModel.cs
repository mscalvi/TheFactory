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
    public double Resistence;
    public int Size;
    public int Tripulation;

    public List<WeaponSlot> WeaponSlots;

    [System.Serializable]
    public class WeaponSlot
    {
        public WeaponModel WeaponModel;
    }

    // Na Instance
    public UnlockHelper.UnlockStatus UnlockStatus;
}
