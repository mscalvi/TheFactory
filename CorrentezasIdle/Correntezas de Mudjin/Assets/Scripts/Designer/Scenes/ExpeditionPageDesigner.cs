using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpeditionPageDesigner : MonoBehaviour
{
    [SerializeField] GameObject ShipPanel;
    [SerializeField] GameObject CrewPanel;
    [SerializeField] GameObject RoomsPanel;
    [SerializeField] GameObject SettingsPanel;

    // Start is called before the first frame update
    void Start()
    {
        // Inicialização dos Paineis
        HideAllMenus();
        ShipPanel.SetActive(true);
    }

    // Troca de Menu de Upgrades
    public void OpenShipMenu()
    {
        HideAllMenus();
        ShipPanel.SetActive(true);
    }

    public void OpenCrewMenu()
    {
        HideAllMenus();
        CrewPanel.SetActive(true);
    }

    public void OpenRoomMenu()
    {
        HideAllMenus();
        RoomsPanel.SetActive(true);
    }

    public void OpenSettingMenu()
    {
        HideAllMenus();
        SettingsPanel.SetActive(true);
    }

    void HideAllMenus()
    {
        ShipPanel.SetActive(false);
        CrewPanel.SetActive(false);
        RoomsPanel.SetActive(false);
        SettingsPanel.SetActive(false);
    }
}
