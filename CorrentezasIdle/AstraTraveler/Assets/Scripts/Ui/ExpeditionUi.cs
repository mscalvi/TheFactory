using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static CurrencyHelper;

public class ExpeditionUi : MonoBehaviour
{
    private GameState GameState;
    private PurchaseService PurchaseService;
    private ConfigurationsService ConfigurationsService;

    [SerializeField] GameObject ShipPanel;
    [SerializeField] GameObject CrewPanel;
    [SerializeField] GameObject ItensPanel;
    [SerializeField] GameObject SettingsPanel;

    [SerializeField] TextMeshProUGUI DaysPastText;
    [SerializeField] Slider DaysCycleSlider;

    [SerializeField] TextMeshProUGUI MissionsText;

    [SerializeField] TextMeshProUGUI PathText;
    [SerializeField] TextMeshProUGUI BiomeText;
    [SerializeField] TextMeshProUGUI ChangeText;

    [SerializeField] TextMeshProUGUI CurrentLifeText;
    [SerializeField] Slider LifeSlider;

    [SerializeField] TextMeshProUGUI GameSpeedText;

    [SerializeField] TMP_Dropdown LanguageDropdown;

    [SerializeField] Transform CurrencyPanel;
    [SerializeField] ExpeditionCurrencyDefinition CurrencyPrefab;
    Dictionary<CurrencyType, ExpeditionCurrencyDefinition> currencyUi = new();

    [SerializeField] Transform ExpeditionShipUpgradesPanel;
    [SerializeField] Transform ExpeditionCrewUpgradesPanel;
    [SerializeField] Transform ExpeditionItensUpgradesPanel;
    [SerializeField] ExpeditionUpgradeDefinition UpgradePrefab;
    Dictionary<string, ExpeditionUpgradeDefinition> shipUpgradeUI = new();

    float MissionsTextTimer = 2000f;
    float MissionsTimer = 0;
    bool MissionTextShown = false;

    public void Initialize(GameState gameState, PurchaseService purchaseService, ConfigurationsService configs)
    {               
        GameState = gameState;

        PurchaseService = purchaseService;

        ConfigurationsService = configs;
    }

    void Start()
    {
        // Inicialização dos Paineis
        HideAllMenus();
        ShipPanel.SetActive(true);

        DayCycleTextSet();
        PathChangeSet();
        LifeTextSet();

        LanguagesDropdownSet();
        GameSpeedText.text = GameState.ActualGameSpeed.ToString();
    }

    private void Update()
    {
        if (MissionTextShown)
        {
            MissionsTimer++;
        }

        if (MissionsTimer >= MissionsTextTimer)
        {
            MissionsTimer = 0;
            MissionsTextSet();
        }

        float phaseProgress = GameState.ExpeditionState.phaseTimer / GameState.ExpeditionState.PhaseDuration;

        if (GameState.ExpeditionState.IsDay)
        {
            DaysCycleSlider.value = phaseProgress * 0.5f;
        }
        else
        {
            DaysCycleSlider.value = 0.5f + (phaseProgress * 0.5f);
        }

        float lifeProgress = 1 - (float)(GameState.ExpeditionState.Ship.CurrentLife / GameState.ExpeditionState.Ship.ActualLife);

        LifeSlider.value = lifeProgress;

        DayCycleTextSet();
        PathChangeSet();
        LifeTextSet();
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
        ItensPanel.SetActive(true);
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
        ItensPanel.SetActive(false);
        SettingsPanel.SetActive(false);
    }

    // Changers
    private void DayCycleTextSet()
    {
        DaysPastText.text = GameState.ExpeditionState.DayCounter.ToString("N0") + " -> " + GameState.ExpeditionState.NextDestination.ToString("N0");
    }
    private void LifeTextSet()
    {
        CurrentLifeText.text = GameState.ExpeditionState.Ship.CurrentLife.ToString("N0");
    }
    private void PathChangeSet()
    {
        var path = GameState.ExpeditionState.ActualPath;

        var language = GameState.ActualLanguage;

        PathText.text = PathHelper.GetPathTypeName(path.Type, language);

        BiomeText.text = PathHelper.GetEnvironmentName(path.Environment.Value, language);

        ChangeText.text = PathHelper.GetModifierName(path.Modifier.Value, language);
    }
    private void CurrencySet(CurrencyType type)
    {
        var currencies = GameState.DataState.currencies;

        if (!currencies.TryGetValue(type, out var currency))
            return;

        if (currency.Scope == CurrencyScope.Expedition)
        {
            if (!currencyUi.TryGetValue(type, out var ui))
                return;

            ui.Setup(currency, GameState.DataState);
        }
    }
    private void UpgradesSet(CurrencyHelper.CurrencyType type)
    {
        foreach (var upgrade in GameState.DataState.upgrades)
        {
            if (upgrade.Value.Currency != type)
                continue;

            if (!shipUpgradeUI.TryGetValue(upgrade.Value.Id, out var ui))
                continue;

            ui.Setup(upgrade.Value, PurchaseService, GameState);            
        }
    }
    private void UpgradeSet(UpgradeInstance upgrade)
    {
        if (!shipUpgradeUI.TryGetValue(upgrade.Id, out var ui))
            return;

        ui.Setup(upgrade, PurchaseService, GameState);
    }
    private void MissionsTextSet()
    {
        Debug.Log("Texto de Missões Limpo");
        MissionTextShown = false;
        MissionsText.text = "";
    }
    private void MissionUpdate(MissionInstance mission)
    {
        if (GameState.ActualLanguage == GameState.Language.Portugues)
        {
            MissionsText.text = "Missão " + mission.NamePT + " Finalizada!";
        }
        if (GameState.ActualLanguage == GameState.Language.English)
        {
            MissionsText.text = "Mission " + mission.NameEN + " Finished!";
        }

        Debug.Log("Texto de Missões Preenchido");
        MissionTextShown = true;
    }


    // Starter Builders
    private void CurrenciesBuild()
    {
        BuildCurrencies(CurrencyPanel);
    }
    private void BuildCurrencies(Transform parent)
    {
        var currencies = GameState.DataState.currencies;

        foreach (Transform child in parent)
            Destroy(child.gameObject);

        currencyUi.Clear();

        var ordered = new List<CurrencyInstance>();

        foreach (var pair in currencies)
        {
            var c = pair.Value;

            if (c.Scope != CurrencyScope.Expedition)
                continue;

            ordered.Add(c);
        }

        ordered.Sort((a, b) => string.Compare(a.Id, b.Id));

        foreach (var currency in ordered)
        {
            var obj = Instantiate(CurrencyPrefab, parent);

            var ui = obj.GetComponent<ExpeditionCurrencyDefinition>();
            ui.Setup(currency, GameState.DataState);
            
            currencyUi[currency.Type] = ui;
        }
    }
    private void BuildShipUpgrades(Transform parent)
    {
        var upgrades = GameState.DataState.upgrades;

        foreach (Transform child in parent)
            Destroy(child.gameObject);

        foreach (var upgrade in upgrades)
        {
            if (upgrade.Value.UnlockStatus != UnlockHelper.UnlockStatus.Available)
                continue;

            if (upgrade.Value.Scope != UpgradeHelper.UpgradeScope.Expedition)
                continue;

            if (upgrade.Value.ExpeditionMenu != UpgradeHelper.UpgradeMenu.Ship)
                continue;

            var obj = Instantiate(UpgradePrefab, parent);
            var ui = obj.GetComponent<ExpeditionUpgradeDefinition>();

            ui.Setup(upgrade.Value, PurchaseService, GameState);

            shipUpgradeUI[upgrade.Value.Id] = ui;
        }
    }
    private void BuildCrewUpgrades(Transform parent)
    {
        var upgrades = GameState.DataState.upgrades;

        foreach (Transform child in parent)
            Destroy(child.gameObject);

        foreach (var upgrade in upgrades)
        {
            if (upgrade.Value.UnlockStatus != UnlockHelper.UnlockStatus.Available)
                continue;

            if (upgrade.Value.Scope != UpgradeHelper.UpgradeScope.Expedition)
                continue;

            if (upgrade.Value.ExpeditionMenu != UpgradeHelper.UpgradeMenu.Crew)
                continue;

            var obj = Instantiate(UpgradePrefab, parent);
            var ui = obj.GetComponent<ExpeditionUpgradeDefinition>();

            ui.Setup(upgrade.Value, PurchaseService, GameState);

            shipUpgradeUI[upgrade.Value.Id] = ui;
        }
    }
    private void BuildItensUpgrades(Transform parent)
    {
        var upgrades = GameState.DataState.upgrades;

        foreach (Transform child in parent)
            Destroy(child.gameObject);

        foreach (var upgrade in upgrades)
        {
            if (upgrade.Value.UnlockStatus != UnlockHelper.UnlockStatus.Available)
                continue;

            if (upgrade.Value.Scope != UpgradeHelper.UpgradeScope.Expedition)
                continue;

            if (upgrade.Value.ExpeditionMenu != UpgradeHelper.UpgradeMenu.Itens)
                continue;

            var obj = Instantiate(UpgradePrefab, parent);
            var ui = obj.GetComponent<ExpeditionUpgradeDefinition>();

            ui.Setup(upgrade.Value, PurchaseService, GameState);

            shipUpgradeUI[upgrade.Value.Id] = ui;
        }
    }


    // Configurações
    public void InreaseSpeedButton()
    {
        ConfigurationsService.IncreaseGameSpeed();

        GameSpeedText.text = GameState.ActualGameSpeed.ToString();
    }
    public void DecreaseSpeedButton()
    {
        ConfigurationsService.DecreaseGameSpeed();

        GameSpeedText.text = GameState.ActualGameSpeed.ToString();
    }
    private void LanguagesDropdownSet()
    {
        LanguageDropdown.ClearOptions();

        foreach (GameState.Language language in System.Enum.GetValues(typeof(GameState.Language)))
        {
            LanguageDropdown.options.Add(
                new TMP_Dropdown.OptionData(language.ToString())
            );
        }

        LanguageDropdown.SetValueWithoutNotify((int)GameState.ActualLanguage);

        LanguageDropdown.RefreshShownValue();
    }
    public void SelectLanguage(int index)
    {
        ConfigurationsService.SelectLanguage(index);
    }
    public void ExitExpedition()
    {
        GameEvents.LifeTest?.Invoke();
    }

    // Eventos
    void OnEnable()
    {
        ExpeditionEvents.OnShipAtributeChange += RefreshShipUi;
        GameEvents.OnUpgradeBuy += RefreshUpgradeUi;
        GameEvents.OnCurrencyChange += RefreshCurrencyUi;
        GameEvents.OnCanBuyChange += RefreshCurrencyUi;
        GameEvents.OnMissionUpdate += MissionUpdate;

        GameEvents.OnLanguageChange += RefreshLanguageUi;

        ExpeditionEvents.OnExpeditionStart += GameStart;
        ExpeditionEvents.OnDayFinish += DayCycleTextSet;
        ExpeditionEvents.OnNightFinish += DayCycleTextSet;

        ExpeditionEvents.OnDestinationArrival += PathChangeSet;
    }

    void OnDisable()
    {
        ExpeditionEvents.OnShipAtributeChange -= RefreshShipUi;
        GameEvents.OnUpgradeBuy -= RefreshUpgradeUi;
        GameEvents.OnCurrencyChange -= RefreshCurrencyUi;
        GameEvents.OnCanBuyChange -= RefreshCurrencyUi;
        GameEvents.OnMissionUpdate -= MissionUpdate;

        GameEvents.OnLanguageChange -= RefreshLanguageUi;

        ExpeditionEvents.OnExpeditionStart -= GameStart;
        ExpeditionEvents.OnDayFinish -= DayCycleTextSet;
        ExpeditionEvents.OnNightFinish -= DayCycleTextSet;

        ExpeditionEvents.OnDestinationArrival -= PathChangeSet;
    }

    void GameStart()
    {
        CurrenciesBuild();
        BuildShipUpgrades(ExpeditionShipUpgradesPanel);
        BuildCrewUpgrades(ExpeditionCrewUpgradesPanel);
        BuildItensUpgrades(ExpeditionItensUpgradesPanel);

        foreach (var upgrade in GameState.DataState.upgrades)
        {
            if (upgrade.Value.UnlockStatus == UnlockHelper.UnlockStatus.Unlocked)
            {
                RefreshUpgradeUi(upgrade.Value);
            }
        }
    }

    void RefreshCurrencyUi(CurrencyType type, CurrencyScope scope)
    {
        CurrencySet(type);
        UpgradesSet(type);
    }

    void RefreshShipUi()
    {
        LifeTextSet();
    }

    void RefreshUpgradeUi(UpgradeInstance upgrade)
    {
        UpgradeSet(upgrade);
    }

    private void RefreshLanguageUi()
    {
        UpgradesSet(CurrencyType.Experience);
    }
}
