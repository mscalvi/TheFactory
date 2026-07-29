using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MissionDefinition : MonoBehaviour
{
    public TMP_Text Name;
    public TMP_Text Description;
    public TMP_Text Progress;

    public void Setup(MissionRuntime mission, GameState GameState)
    {
        if (GameState.ActualLanguage == GameState.Language.English)
        {
            Name.text = mission.NameEN;

            Description.text = mission.DescriptionEN;
        }

        if (GameState.ActualLanguage == GameState.Language.Portugues)
        {
            Name.text = mission.NamePT;

            Description.text = mission.DescriptionPT;
        }


        double progress = mission.TargetValue - mission.ActualValue;

        Progress.text = progress.ToString("N0");
    }
}
