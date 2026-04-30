using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MissionDefinition : MonoBehaviour
{
    public TMP_Text Name;
    public TMP_Text Description;
    public TMP_Text Progress;

    public void Setup(MissionInstance mission)
    {
        Name.text = mission.Name;

        Description.text = mission.Description;

        double progress = mission.TargetValue - mission.CurrentValue;

        Progress.text = progress.ToString("N0");
    }
}
