using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TripulationDefinition : MonoBehaviour
{
    public TMP_Text Name;

    private TripulationInstance tripulation;
    private UnlockService UnlockService;
    private TripulationUi Ui;

    public void Setup(TripulationInstance tripulationInstance, UnlockService unlockService, TripulationUi ui)
    {
        tripulation = tripulationInstance;

        UnlockService = unlockService;

        Ui = ui;

        Name.text = tripulation.Name;
    }

    public void OnClick()
    {
        Ui.ShowTraining(tripulation);
    }
}
