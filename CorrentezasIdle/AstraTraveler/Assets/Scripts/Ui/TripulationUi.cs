using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static CurrencyHelper;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class TripulationUi : MonoBehaviour
{
    private GameState GameState;
    private DataState DataState;

    private UnlockService UnlockService;
    private RecruitmentService RecruitmentService;
    private PurchaseService PurchaseService;
    private CurrencyService CurrencyService;

    public TMP_Text Name;
    public TMP_Text TripulationName;
    
    public Button CancelButton;

    [SerializeField] Transform Tripulation;
    [SerializeField] TripulationDefinition TripulationPrefab;
    [SerializeField] TripulationButtonDefinition TripulationButtonPrefab;

    [SerializeField] Transform Recruit;
    [SerializeField] RecruitDefinition RecruitPrefab;

    [SerializeField] GameObject TrainingPanel;
    [SerializeField] GameObject RecruitPanel;

    public void Initialize(GameState gameState, UnlockService unlockService, RecruitmentService recruitmentService, PurchaseService purchase, CurrencyService currency)
    {
        GameState = gameState;

        DataState = GameState.DataState;

        UnlockService = unlockService;

        RecruitmentService = recruitmentService;

        PurchaseService = purchase;

        CurrencyService = currency;

        TrainingPanel.SetActive(false);
        RecruitPanel.SetActive(false);

        Populate();
    }

    void Populate()
    {
        ClearContainer();

        int unlockedCount = 0;

        foreach (var tripulation in DataState.tripulations)
        {
            if (tripulation.Value.UnlockStatus != UnlockHelper.UnlockStatus.Unlocked)
                continue;

            unlockedCount++;

            var go = Instantiate(TripulationPrefab, Tripulation);
            var ui = go.GetComponent<TripulationDefinition>();

            ui.Setup(tripulation.Value, UnlockService, this);
        }

        int max = GameState.ExpeditionState.Ship.MaxTripulation;
        int remainingSlots = max - unlockedCount;

        for (int i = 0; i < remainingSlots; i++)
        {
            var go = Instantiate(TripulationButtonPrefab, Tripulation);

            var ui = go.GetComponent<TripulationButtonDefinition>();

            ui.Setup(this);
        }
    }

    void ClearContainer()
    {
        foreach (Transform child in Tripulation)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in Recruit)
        {
            Destroy(child.gameObject);
        }
    }

    public void ShowTraining(TripulationInstance tripulation)
    {
        if (GameState.UnlockState.Training)
            return;

        TrainingPanel.SetActive(true);
        RecruitPanel.SetActive(false);

        TripulationName.text = tripulation.Name;
    }

    public void ShowRecruit()
    {
        if (GameState.UnlockState.Recruiting)
            return;

        TrainingPanel.SetActive(false);
        RecruitPanel.SetActive(true);

        TripulationName.text = "Novo Recruta!";

        RecruitmentService.GenerateRecruitOptions();

        PopulateRecruitOptions();
    }

    void PopulateRecruitOptions()
    {
        foreach (Transform child in Recruit)
        {
            Destroy(child.gameObject);
        }

        var options = GameState.ExpeditionState.Ship.ActiveRecruits;

        foreach (var option in options)
        {
            var go = Instantiate(RecruitPrefab, Recruit);

            var ui = go.GetComponent<RecruitDefinition>();

            ui.Setup(option, PurchaseService);
        }
    }

    public void Cancel()
    {
        var ship = GameState.ExpeditionState.Ship;

        ship.ActiveRecruits.Clear();

        var prestige = GameState.DataState.currencies[CurrencyType.Prestige];

        double loss = Mathf.CeilToInt((float)(prestige.Amount * 0.5f));

        loss = Mathf.Min((float)loss, (float)prestige.Amount);

        CurrencyService.Spend(CurrencyType.Prestige, loss);

        RecruitPanel.SetActive(false);
    }

    public void Return()
    {
        SceneManager.LoadScene("LandingScene");
    }
}
