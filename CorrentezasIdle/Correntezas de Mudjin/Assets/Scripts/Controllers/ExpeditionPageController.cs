using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpeditionPageController : MonoBehaviour
{
    [SerializeField] GameObject shipPanel;
    [SerializeField] GameObject crewPanel;
    [SerializeField] GameObject roomsPanel;
    [SerializeField] GameObject settingsPanel;

    // Start is called before the first frame update
    void Start()
    {
        // Inicialização dos Paineis
        HideAllMenus();
        shipPanel.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Troca de Menu de Upgrades
    public void OpenShipMenu()
    {
        HideAllMenus();
        shipPanel.SetActive(true);
    }
    public void OpenCrewMenu()
    {
        HideAllMenus();
        crewPanel.SetActive(true);
    }
    public void OpenRoomMenu()
    {
        HideAllMenus();
        roomsPanel.SetActive(true);
    }
    public void OpenSettingMenu()
    {
        HideAllMenus();
        settingsPanel.SetActive(true);
    }
    void HideAllMenus()
    {
        shipPanel.SetActive(false);
        crewPanel.SetActive(false);
        roomsPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }
}
