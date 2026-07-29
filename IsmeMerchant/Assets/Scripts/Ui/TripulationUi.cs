using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static CurrencyHelper;

public class TripulationUi : MonoBehaviour
{
    private GameState GameState;
    private DataState DataState;

    private UnlockService UnlockService;
    private RecruitmentService RecruitmentService;
    private PurchaseService PurchaseService;
    private CurrencyService CurrencyService;

    public TMP_Text TripulationName;
    public TMP_Text TripulationClass;
    
    public Button CancelButton;

    [SerializeField] Transform Tripulation;
    [SerializeField] TripulationDefinition TripulationPrefab;
    [SerializeField] TripulationButtonDefinition TripulationButtonPrefab;

    [SerializeField] Transform Recruit;
    [SerializeField] RecruitDefinition RecruitPrefab;

    [SerializeField] Transform CompanyCurrencyPanel;
    [SerializeField] CompanyCurrencyDefinition CurrencyPrefab;
    Dictionary<CurrencyType, CompanyCurrencyDefinition> currencyUi = new();

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

        BuildCurrencies(CompanyCurrencyPanel);
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

        int max = GameState.ExpeditionState.ActualMaxTripulation;
        int remainingSlots = max - unlockedCount;

        for (int i = 0; i < remainingSlots; i++)
        {
            var go = Instantiate(TripulationButtonPrefab, Tripulation);

            var ui = go.GetComponent<TripulationButtonDefinition>();

            ui.Setup(this);
        }
    }

    public void BuildCurrencies(Transform parent)
    {
        var currencies = GameState.DataState.currencies;

        foreach (Transform child in parent)
            Destroy(child.gameObject);

        currencyUi.Clear();

        var ordered = new List<CurrencyInstance>();

        foreach (var pair in currencies)
        {
            var c = pair.Value;

            if (c.Scope != CurrencyScope.Company)
                continue;

            if(c.UnlockStatus != UnlockHelper.UnlockStatus.Unlocked) 
                continue;

            ordered.Add(c);
        }

        ordered.Sort((a, b) => string.Compare(a.Id, b.Id));

        foreach (var currency in ordered)
        {
            var obj = Instantiate(CurrencyPrefab, parent);

            var ui = obj.GetComponent<CompanyCurrencyDefinition>();
            ui.Setup(currency, DataState);

            currencyUi[currency.Type] = ui;
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
        if (GameState.ProgressState.Training)
            return;

        TrainingPanel.SetActive(true);
        RecruitPanel.SetActive(false);

        TripulationName.text = tripulation.Name;
        TripulationClass.text = tripulation.Type.ToString();
    }

    public void ShowRecruit()
    {
        if (!GameState.ProgressState.Recruiting)
            return;

        TrainingPanel.SetActive(false);
        RecruitPanel.SetActive(true);

        TripulationName.text = "Novo Recruta!";
        TripulationClass.text = "";

        RecruitmentService.GenerateRecruitOptions();

        PopulateRecruitOptions();
    }

    void PopulateRecruitOptions()
    {
        foreach (Transform child in Recruit)
        {
            Destroy(child.gameObject);
        }

        var options = GameState.ExpeditionState.ActiveRecruits;

        foreach (var option in options)
        {
            var go = Instantiate(RecruitPrefab, Recruit);

            var ui = go.GetComponent<RecruitDefinition>();

            ui.Setup(option, PurchaseService, GameState);
        }
    }

    public void Cancel()
    {
        var ship = GameState.ExpeditionState.Ship;

        GameState.ExpeditionState.ActiveRecruits.Clear();

        var prestige = GameState.DataState.currencies[CurrencyType.Prestige];

        double loss = Mathf.CeilToInt((float)(prestige.Amount * 0.2f));

        loss = Mathf.Min((float)loss, (float)prestige.Amount);

        CurrencyService.Spend(CurrencyType.Prestige, loss);

        RecruitPanel.SetActive(false);
    }

    public void Return()
    {
        SceneManager.LoadScene("LandingScene");
    }

    private void OnEnable()
    {
        GameEvents.OnTripulationPurchase += RefreshTripulationUi;
    }

    private void OnDisable()
    {
        GameEvents.OnTripulationPurchase -= RefreshTripulationUi;
    }

    private void RefreshTripulationUi(TripulationInstance tripulation)
    {
        Populate();
        BuildCurrencies(CompanyCurrencyPanel);
    }
}
