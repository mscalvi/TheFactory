using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Mission")]
public class MissionModel : ScriptableObject
{
    public string Id;
    public string Name;
    public string Description;

    public double RewardFactor;
    public CurrencyHelper.CurrencyType RewardType1;
    public CurrencyHelper.CurrencyType RewardType2;
    public CurrencyHelper.CurrencyType RewardType3;
    public CurrencyHelper.CurrencyType RewardType4;

    public GameHelper.ItemRarity MissionRarity;

    public MissionHelper.MissionType MissionType;
    public MissionHelper.MissionStatus MissionStatus;

    public string UnlockId;
    public UnlockHelper.UnlockStatus UnlockStatus;
}