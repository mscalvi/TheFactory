using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Mission")]
public class MissionModel : ScriptableObject
{
    public string Id;
    public string Name;
    public double RewardFactor;

    public MissionHelper.MissionRarity MissionRarity;

    public MissionHelper.MissionType MissionType;
    public MissionHelper.MissionStatus MissionStatus;
    public MissionHelper.TargetType TargetType;

    public string UnlockId;
}