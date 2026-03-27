using Unity.VisualScripting;
using UnityEngine;

public class WeaponInstance
{
    public WeaponModel Model;

    public string Id;
    public string Name;
    public string Description;

    public double Damage;
    public double Range;
    public double AttackSpeed;
    public double Precision;
    public double CriticalDamage;
    public string Special;
    public WeaponHelper.AmmoType AmmoType;

    public UnlockHelper.UnlockStatus UnlockStatus;

    public WeaponInstance(WeaponModel model)
    {
        Id = model.Id;
        Name = model.Name;
        Description = model.Description;

        Damage = model.Damage;
        Range = model.Range;
        AttackSpeed = model.AttackSpeed;
        Precision = model.Precision;
        CriticalDamage = model.CriticalDamage;
        Special = model.Special;
        AmmoType = model.AmmoType;

        UnlockStatus = model.UnlockStatus;
    }
}