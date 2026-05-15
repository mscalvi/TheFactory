using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcquisitionModel
{
    public string Id;

    public string NamePT;
    public string NameEN;
    public string DescriptionPT;
    public string DescriptionEN;

    public TripulationHelper.Type Type;
    public UpgradeHelper.TargetType TargetType;

    public double Cost;
    public CurrencyHelper.CurrencyType Currency;

    public float Time;

    public string UnlockId;
    public UnlockHelper.UnlockStatus UnlockStatus;
}
