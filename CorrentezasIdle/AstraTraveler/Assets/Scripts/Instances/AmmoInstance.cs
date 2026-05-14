using Unity.VisualScripting;
using UnityEngine;

public class AmmoInstance
{
    public AmmoModel Model;

    public string Id;
    public string Name;
    public string Description;

    public WeaponHelper.AmmoType Type;

    public double Damage;
    public string Special;

    public ProjectileModel Projectile;

    public UnlockHelper.UnlockStatus UnlockStatus;

    public AmmoInstance(AmmoModel model)
    {
        Id = model.Id;

        Damage = model.Damage;

        Projectile = model.Projectile;

        UnlockStatus = model.UnlockStatus;
    }
}