using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyModel
{
    public string Id;

    public string NamePT;
    public string NameEN;
    public string DescriptionPT;
    public string DescriptionEN;

    public CurrencyHelper.CurrencyType Type;
    public CurrencyHelper.CurrencyScope Scope;

    public string UnlockId;
    public UnlockHelper.UnlockStatus UnlockStatus;
}