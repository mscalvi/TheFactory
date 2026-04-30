using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionButtonDefinition : MonoBehaviour
{
    public Button Button;
    public TMP_Text Info;

    private LandingUi Ui;
    private MissionSlotModel Slot;

    public void Setup(LandingUi uiService, MissionSlotModel missionSlot)
    {
        Ui = uiService;
        Slot = missionSlot;

        Button.onClick.RemoveAllListeners();
        Button.onClick.AddListener(OnClick);

        long now = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        bool isOnCooldown = Slot.CooldownEnd > now;

        Button.interactable = !isOnCooldown;
    }

    public void SetupAvailable(LandingUi service, MissionSlotModel slot)
    {
        Button.interactable = true;

        Button.onClick.RemoveAllListeners();
        Button.onClick.AddListener(() => service.SelectNewMission(slot));
    }

    public void SetupCooldown(MissionSlotModel slot)
    {
        Button.interactable = false;

        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        long remaining = slot.CooldownEnd - now;
    }

    void OnClick()
    {
        Ui.SelectNewMission(Slot);
    }
}