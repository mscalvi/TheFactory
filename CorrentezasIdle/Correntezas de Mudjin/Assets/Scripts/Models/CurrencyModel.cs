using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Currency")]
public class CurrencyModel : ScriptableObject
{
    public string Id;
    public CurrencyHelper.CurrencyType Type;
    public CurrencyHelper.CurrencyScope Scope;
    public string Image;
    public string Logo;

    public UnlockHelper.UnlockStatus UnlockStatus;
}