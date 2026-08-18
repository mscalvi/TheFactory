using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static CurrencyHelper;

public class LandingUi : MonoBehaviour
{
    private GameState GameState;

    private MissionsService MissionsService;
    private UnlockService UnlockService;
    private PurchaseService PurchaseService;
    private AlchemyService AlchemyService;
    private IngredientService IngredientService;
    private TutorialService TutorialService;

    [SerializeField] GameObject TutorialPopUp;
    [SerializeField] TextMeshProUGUI TutorialTitleText;
    [SerializeField] TextMeshProUGUI TutorialInfoText;

    [SerializeField] Button ExpeditionButton;
    [SerializeField] TextMeshProUGUI RecordText;

    [SerializeField] GameObject BigBtnPanel;
    [SerializeField] GameObject SmallBtnPanel;

    [SerializeField] UpgradesPopUp UpgradesPopUp;
    [SerializeField] BuildingsPopUp BuildingsPopUp;
    [SerializeField] BestiaryPopUp BestiaryPopUp;
    [SerializeField] AlchemyPopUp AlchemyPopUp;

    [SerializeField] GameObject ConfigsPopUp;
    [SerializeField] Transform MissionPopUp;

    Dictionary<CurrencyType, CompanyCurrencyDefinition> currencyUi = new();
    [SerializeField] Transform CompanyCurrencyPanel;
    [SerializeField] CompanyCurrencyDefinition CurrencyPrefab;

    Dictionary<AlchemyHelper.IngredientType, CompanyIngredientDefinition> ingredientsUi = new();
    [SerializeField] Transform IngredientPanel;
    [SerializeField] CompanyIngredientDefinition IngredientPrefab;

    private Queue<UpgradeHelper.UpgradeScope> upgradeQueue = new();

    public void Initialize(GameState gameState, MissionsService missionsService, UnlockService unlockService, PurchaseService purchaseSercice, AlchemyService alchemy, IngredientService ingredients, TutorialService tutorial)
    {
        GameState = gameState;
        MissionsService = missionsService;
        UnlockService = unlockService;
        PurchaseService = purchaseSercice;
        AlchemyService = alchemy;
        IngredientService = ingredients;
        TutorialService = tutorial;

        BlockButtons();
        BuildCurrencies(CompanyCurrencyPanel);
        BuildIngredients(IngredientPanel);
        CheckUpgrades();
        ReleaseButtons();
        BuildRecord();

        if (!GameState.ProgressState.StartTut)
        {
            ShowTutorial(GameHelper.Tutorial.StartTut);

            return;
        }
    }

    // Buttons
    private void BlockButtons()
    {
        SmallBtnPanel.SetActive(false);
        BigBtnPanel.SetActive(false);
    }

    private void ReleaseButtons()
    {
        var Unlock = GameState.ProgressState;

        if (GameState.ExpeditionState.ExpeditionStatus == GameHelper.ExpeditionStatus.Running)
            return;

        if (GameState.ExpeditionState.ExpeditionsDone < 1) 
            return;

        SmallBtnPanel.SetActive(true);
        BigBtnPanel.SetActive(true);
    }    

    public void BotaoSecretoDaGrana()
    {
        GameEvents.MoneyTest?.Invoke();
    }

    public void ExpeditionButtonFunction()
    {
        SceneManager.LoadScene("ExpeditionScene");
    }

    public void BestiaryButtonFuncion()
    {
        if (!GameState.ProgressState.BestiaryTut)
        {
            ShowTutorial(GameHelper.Tutorial.BestiaryTut);

            return;
        }

        BestiaryPopUp.Show(GameState);
    }

    public void RoomsButtonFuncion()
    {
        if (!GameState.ProgressState.BuildingsTut)
        {
            ShowTutorial(GameHelper.Tutorial.BuildingsTut);

            return;
        }

        BuildingsPopUp.Show(GameState, PurchaseService);
    }
    public void AlchemyButtonFunction()
    {
        if (!GameState.ProgressState.AlchemyTut)
        {
            ShowTutorial(GameHelper.Tutorial.AlchemyTut);

            return;
        }

        AlchemyPopUp.Show(GameState, AlchemyService, IngredientService);
    }


    // Tutorial
    private void ShowTutorial(GameHelper.Tutorial type)
    {
        BestiaryPopUp.Hide();
        AlchemyPopUp.Hide();
        BuildingsPopUp.Hide();

        var text = TutorialService.SetText(type);

        TutorialTitleText.text = text.Item1;
        Debug.Log(text.Item1);
        Debug.Log(text.Item2);
        TutorialInfoText.text = text.Item2;

        TutorialPopUp.SetActive(true);
    }
    public void CloseTutorial()
    {
        TutorialPopUp.SetActive(false);
    }


    // Currency
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

            if (c.UnlockStatus != UnlockHelper.UnlockStatus.Unlocked)
                continue;

            ordered.Add(c);
        }

        ordered.Sort((a, b) => string.Compare(a.Id, b.Id));

        foreach (var currency in ordered)
        {
            var obj = Instantiate(CurrencyPrefab, parent);

            var ui = obj.GetComponent<CompanyCurrencyDefinition>();
            ui.Setup(currency, GameState.DataState);

            currencyUi[currency.Type] = ui;
        }
    }
    public void BuildIngredients(Transform parent)
    {
        var currencies = GameState.DataState.ingredients;

        foreach (Transform child in parent)
            Destroy(child.gameObject);

        ingredientsUi.Clear();

        var ordered = new List<IngredientInstance>();

        foreach (var pair in currencies)
        {
            var c = pair.Value;

            if (c.UnlockStatus != UnlockHelper.UnlockStatus.Unlocked && c.UnlockStatus != UnlockHelper.UnlockStatus.Available)
                continue;

            ordered.Add(c);
        }

        ordered.Sort((a, b) => string.Compare(a.Id, b.Id));

        foreach (var ingredient in ordered)
        {
            var obj = Instantiate(IngredientPrefab, parent);

            var ui = obj.GetComponent<CompanyIngredientDefinition>();
            ui.Setup(ingredient, GameState.DataState);

            ingredientsUi[ingredient.Type] = ui;
        }
    }


    // Upgrades
    private void CheckUpgrades()
    {
        upgradeQueue.Clear();

        for (int i = 0; i < GameState.ProgressState.UnlockableExpeditionUpgrades; i++)
        {
            upgradeQueue.Enqueue(UpgradeHelper.UpgradeScope.Expedition);
        }

        for (int i = 0; i < GameState.ProgressState.UnlockableCompanyUpgrades; i++)
        {
            upgradeQueue.Enqueue(UpgradeHelper.UpgradeScope.Company);
        }

        GameState.ProgressState.UnlockableExpeditionUpgrades = 0;
        GameState.ProgressState.UnlockableCompanyUpgrades = 0;

        ShowNextUpgrade();
    }
    private void ShowNextUpgrade()
    {
        if (upgradeQueue.Count == 0)
            return;

        var scope = upgradeQueue.Dequeue();

        var options = UnlockService.UpgradeOptions(scope);

        if (options == null || options.Count == 0)
        {
            ShowNextUpgrade();
            return;
        }

        UpgradesPopUp.ShowUpgrades(options, (selected) =>
        {
            selected.UnlockStatus = UnlockHelper.UnlockStatus.Available;

            UpgradesPopUp.Hide();

            ShowNextUpgrade();
        }, GameState);
    }


    // Ui
    private void BuildRecord()
    {
        if (GameState.ActualLanguage == GameState.Language.Portugues)
        {
            RecordText.text = GameState.ProgressState.MaxDaysTraveling.ToString() + " dias";
        }

        if (GameState.ActualLanguage == GameState.Language.English)
        {
            RecordText.text = GameState.ProgressState.MaxDaysTraveling.ToString() + " days";
        }
    }


    // Eventos
    void OnEnable()
    {
        GameEvents.OnCurrencyChange += RefreshCurrencyUi;
        GameEvents.OnCanBuyChange += RefreshCurrencyUi;
        GameEvents.OnIngredientChange += RefreshIngredientUi;
    }

    void OnDisable()
    {
        GameEvents.OnCurrencyChange -= RefreshCurrencyUi;
        GameEvents.OnCanBuyChange -= RefreshCurrencyUi;
        GameEvents.OnIngredientChange += RefreshIngredientUi;
    }

    void RefreshCurrencyUi(CurrencyType type, CurrencyScope scope)
    {
        BuildCurrencies(CompanyCurrencyPanel);
        BuildIngredients(IngredientPanel);
    }
    void RefreshIngredientUi(AlchemyHelper.IngredientType type)
    {
        BuildIngredients(IngredientPanel);
    }
}