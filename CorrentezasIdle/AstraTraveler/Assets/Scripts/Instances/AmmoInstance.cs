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

    public UnlockHelper.UnlockStatus UnlockStatus;

    public AmmoInstance(AmmoModel model)
    {
        Id = model.Id;
        Name = model.Name;
        Description = model.Description;

        Damage = model.Damage;
        Special = model.Special;

        UnlockStatus = model.UnlockStatus;
    }
}