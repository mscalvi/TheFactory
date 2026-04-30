using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyInstance
{
    public CurrencyModel Model;

    public string Id;

    public CurrencyHelper.CurrencyType Type;
    public CurrencyHelper.CurrencyScope Scope;

    public double Amount;

    public UnlockHelper.UnlockStatus UnlockStatus;

    public CurrencyInstance(CurrencyModel model)
    {
        Id = model.Id;
        Type = model.Type;
        Scope = model.Scope;
        Amount = 0;

        UnlockStatus = model.UnlockStatus;
    }

    public CurrencyInstance(CurrencyInstance model)
    {
        Id = model.Id;
        Type = model.Type;
        Scope = model.Scope;
        Amount = 0;

        UnlockStatus = model.UnlockStatus;
    }
}
