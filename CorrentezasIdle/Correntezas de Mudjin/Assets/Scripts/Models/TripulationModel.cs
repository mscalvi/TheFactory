using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Tripulation")]
public class TripulationModel : ScriptableObject
{
    public string Id;
    public string Name;
    public string Description;

    public double Str;
    public double Dex;
    public double Int;
    public double Luk;
    public double Cha;
    public double Wis;

    public TripulationHelper.Type Type;
    public TripulationHelper.Jobs Jobs;

    public string UnlockId;
    public UnlockHelper.UnlockStatus UnlockStatus;
}
