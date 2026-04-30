using System;
using UnityEngine;
using UnityEngine.UI;

public class SecondaryMissionButtonDefinition : MonoBehaviour
{
    public Button Button;

    private LandingUiService LandingUiService;
    private MissionSlotModel Slot;

    public void Setup(LandingUiService uiService, MissionSlotModel missionSlot)
    {
        LandingUiService = uiService;
        Slot = missionSlot;

        Button.onClick.RemoveAllListeners();
        Button.onClick.AddListener(OnClick);

        long now = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        bool isOnCooldown = Slot.CooldownEnd > now;

        Button.interactable = !isOnCooldown;
    }

    public void SetupAvailable(LandingUiService service, MissionSlotModel slot)
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
        LandingUiService.SelectNewMission(Slot);
    }
}