using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TripulationButtonDefinition : MonoBehaviour
{
    public Button Button;

    private TripulationUi Ui;

    public void Setup(TripulationUi ui)
    {
        Ui = ui;
    }

    public void OnRecruitClicked()
    {
        Ui.ShowRecruit();
    }
}
