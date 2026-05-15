using Unity.VisualScripting;
using UnityEngine;

public class TripulationInstance
{
    public TripulationModel Model;

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

    public TripulationInstance(TripulationModel model)
    {
        Id = model.Id;
        Name = model.Name;
        DescriptionPT = model.DescriptionPT;
        DescriptionEN = model.DescriptionEN;

        Str = model.Str;
        Dex = model.Dex;
        Int = model.Int;
        Luk = model.Luk;
        Cha = model.Cha;
        Wis = model.Wis;

        Type = model.Type;
        Rarity = model.Rarity;

        UnlockId = model.UnlockId;
        UnlockStatus = model.UnlockStatus;
    }
}