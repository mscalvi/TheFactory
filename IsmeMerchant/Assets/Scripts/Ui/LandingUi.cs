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

    [SerializeField] Button ExpeditionButton;
    [SerializeField] TextMeshProUGUI RecordText;

    [SerializeField] GameObject BigBtnPanel;
    [SerializeField] GameObject SmallBtnPanel;

    [SerializeField] UpgradesPopUp UpgradesPopUp;
    [SerializeField] BuildingsPopUp BuildingsPopUp;
    [SerializeField] BestiaryPopUp BestiaryPopUp;

    [SerializeField] GameObject ConfigsPopUp;
    [SerializeField] Transform MissionPopUp;

    Dictionary<CurrencyType, CompanyCurrencyDefinition> currencyUi = new();
    [SerializeField] Transform CompanyCurrencyPanel;
    [SerializeField] CompanyCurrencyDefinition CurrencyPrefab;

    public void Initialize(GameState gameState, MissionsService missionsService, UnlockService unlockService, PurchaseService purchaseSercice)
    {
        GameState = gameState;
        MissionsService = missionsService;
        UnlockService = unlockService;
        PurchaseService = purchaseSercice;

        BlockButtons();
        BuildCurrencies(CompanyCurrencyPanel);
        CheckUpgrades();
        ReleaseButtons();
        BuildRecord();
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
        BestiaryPopUp.Show(GameState);
    }
    public void RoomsButtonFuncion()
    {
        BuildingsPopUp.Show(GameState, PurchaseService);
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

    // Upgrades
    private void CheckUpgrades()
    {
        do
        {
            Debug.Log($"Checando Upgrades: {GameState.ProgressState.UnlockableExpeditionUpgrades}/{GameState.ProgressState.UnlockableCompanyUpgrades}");
            if (GameState.ProgressState.UnlockableExpeditionUpgrades > 0)
            {
                Debug.Log("Encontrado Upgrade de Expedição");
                for (int i = GameState.ProgressState.UnlockableExpeditionUpgrades; i > 0; i--)
                {
                    var expUpgradesOptions = UnlockService.UpgradeOptions(UpgradeHelper.UpgradeScope.Expedition);

                    UpgradesPopUp.ShowUpgrades(expUpgradesOptions, (selected) =>
                    {
                        selected.UnlockStatus = UnlockHelper.UnlockStatus.Available;

                        UpgradesPopUp.Hide();
                    }, GameState);
                }
            }
            if (GameState.ProgressState.UnlockableCompanyUpgrades > 0)
            {
                Debug.Log("Encontrado Upgrade de Companhia");
                for (int i = GameState.ProgressState.UnlockableCompanyUpgrades; i > 0; i--)
                {
                    var compUpgradesOptions = UnlockService.UpgradeOptions(UpgradeHelper.UpgradeScope.Company);

                    UpgradesPopUp.ShowUpgrades(compUpgradesOptions, (selected) =>
                    {
                        selected.UnlockStatus = UnlockHelper.UnlockStatus.Available;

                        UpgradesPopUp.Hide();
                    }, GameState);
                }
            }

            GameState.ProgressState.UnlockableExpeditionUpgrades--;
            GameState.ProgressState.UnlockableCompanyUpgrades--;

            Debug.Log($"Faltam Upgrades de Companhia: {GameState.ProgressState.UnlockableCompanyUpgrades}");
            Debug.Log($"Faltam Upgrades de Expedição: {GameState.ProgressState.UnlockableExpeditionUpgrades}");

        } while (GameState.ProgressState.UnlockableExpeditionUpgrades > 0 && GameState.ProgressState.UnlockableCompanyUpgrades > 0);

        GameState.ProgressState.UnlockableExpeditionUpgrades = 0;
        GameState.ProgressState.UnlockableCompanyUpgrades = 0;
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
    }

    void OnDisable()
    {
        GameEvents.OnCurrencyChange -= RefreshCurrencyUi;
        GameEvents.OnCanBuyChange -= RefreshCurrencyUi;
    }

    void RefreshCurrencyUi(CurrencyType type, CurrencyScope scope)
    {
        BuildCurrencies(CompanyCurrencyPanel);
    }
}