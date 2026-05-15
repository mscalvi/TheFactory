using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionModel
{
    public string Id;

    public string NamePT;
    public string NameEN;

    public string DescriptionPT;
    public string DescriptionEN;

    public int Level;
    public GameHelper.ItemRarity MissionRarity;

    public CurrencyHelper.CurrencyType RewardType1;
    public CurrencyHelper.CurrencyType RewardType2;
    public CurrencyHelper.CurrencyType RewardType3;
    public CurrencyHelper.CurrencyType RewardType4;

    public MissionHelper.MissionType MissionType;

    public string UnlockId;
    public UnlockHelper.UnlockStatus UnlockStatus;
}