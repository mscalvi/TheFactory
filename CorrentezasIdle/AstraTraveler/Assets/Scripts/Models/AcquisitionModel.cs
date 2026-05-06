using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Acquisition")]

public class AcquisitionModel : ScriptableObject
{
    public string Id;
    public string Name;
    public string Description;

    public UpgradeHelper.TargetType TargetType;
    public string TargetId;
    public string UnlockId;

    public double Cost;
    public CurrencyHelper.CurrencyType Currency;

    public float Time;

    public UnlockHelper.UnlockStatus UnlockStatus;
}
