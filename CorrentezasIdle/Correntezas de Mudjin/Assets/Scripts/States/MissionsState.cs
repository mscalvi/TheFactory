using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionsState
{
    // Progresso das Missões Secondárias
    //public List<MissionInstance> ActiveMissions = new List<MissionInstance>();

    public int MaxOnGoingMissions = 0;
    public int MaxRewardItens = 1;
    public double RewardBonus = 1;
    public int MaxMissionsOptions = 1;
    public int MaxCancelableMissions = 0;

    public int CompletedMissions;
    public int CanceledMissions;

    public List<MissionSlotModel> Slots = new List<MissionSlotModel>();
    public int MaxSlots = 4;
}