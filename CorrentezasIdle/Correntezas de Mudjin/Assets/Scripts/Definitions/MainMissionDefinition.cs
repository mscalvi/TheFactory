using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMissionDefiniton : MonoBehaviour
{
    public TMP_Text Name;
    public TMP_Text Description;
    public TMP_Text CurrentProgress;
    public TMP_Text TargetProgress;

    public void Setup(MissionInstance mission)
    {
        Name.text = mission.Name;
        Description.text = mission.Description;

        CurrentProgress.text = mission.CurrentValue.ToString("N0");
        TargetProgress.text = mission.TargetValue.ToString("N0");
    }
}
