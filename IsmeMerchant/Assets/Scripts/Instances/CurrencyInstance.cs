using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyInstance
{
    public CurrencyModel Model;

    public string Id;
    public string NamePT;
    public string NameEN;
    public string DescriptionPT;
    public string DescriptionEN;

    public CurrencyHelper.CurrencyType Type;
    public CurrencyHelper.CurrencyScope Scope;

    public double Amount;
    public int VisualAmount => (int)Amount;

    public string UnlockId;
    public UnlockHelper.UnlockStatus UnlockStatus;

    public CurrencyInstance(CurrencyModel model)
    {
        Id = model.Id;
        NamePT = model.NamePT;
        NameEN = model.NameEN;
        DescriptionPT = model.DescriptionPT;
        DescriptionEN = model.DescriptionEN;

        Type = model.Type;
        Scope = model.Scope;

        Amount = 0;

        UnlockId = model.UnlockId;
        UnlockStatus = model.UnlockStatus;
    }

    public CurrencyInstance()
    {

    }
}
