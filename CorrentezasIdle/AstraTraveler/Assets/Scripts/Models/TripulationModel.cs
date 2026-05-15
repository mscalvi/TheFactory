using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripulationModel
{
    public string Id;
    public string Name;
    public string DescriptionPT;
    public string DescriptionEN;

    public double Str;
    public double Dex;
    public double Int;
    public double Luk;
    public double Cha;
    public double Wis;

    public TripulationHelper.Type Type;
    public GameHelper.ItemRarity Rarity;

    public string UnlockId;
    public UnlockHelper.UnlockStatus UnlockStatus;
}
