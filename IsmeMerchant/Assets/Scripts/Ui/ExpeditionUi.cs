using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using static CurrencyHelper;

public class ExpeditionUi : MonoBehaviour
{
    private GameState GameState;
    private PurchaseService PurchaseService;
    private ConfigurationsService ConfigurationsService;
    private TutorialService TutorialService;
    private TickService TickService;

    [SerializeField] GameObject ShipPanel;
    [SerializeField] GameObject CrewPanel;
    [SerializeField] GameObject ItensPanel;
    [SerializeField] GameObject WeaponsPanel;
    [SerializeField] GameObject SettingsPanel;

    [SerializeField] TextMeshProUGUI DaysPastText;
    [SerializeField] Slider DaysCycleSlider;

    [SerializeField] TextMeshProUGUI MessagesText;

    [SerializeField] TextMeshProUGUI PathText;
    [SerializeField] TextMeshProUGUI BiomeText;
    [SerializeField] TextMeshProUGUI ChangeText;

    [SerializeField] GameObject LifePanel;
    [SerializeField] GameObject InfoPanel;
    private bool InfoPanelActive = false;
    [SerializeField] TextMeshProUGUI CurrentLifeText;
    [SerializeField] Slider LifeSlider;

    [SerializeField] TextMeshProUGUI ShipNameText;
    [SerializeField] TextMeshProUGUI WeaponsText;
    [SerializeField] TextMeshProUGUI RegenText;
    [SerializeField] TextMeshProUGUI ArmorText;
    [SerializeField] TextMeshProUGUI BestTravelText;
    [SerializeField] TextMeshProUGUI MarcosDayText;
    [SerializeField] TextMeshProUGUI ExperienceDayText;
    [SerializeField] TextMeshProUGUI DefenseText;

    [SerializeField] TextMeshProUGUI GameSpeedText;

    [SerializeField] TMP_Dropdown LanguageDropdown;

    [SerializeField] Transform CurrencyPanel;
    [SerializeField] ExpeditionCurrencyDefinition CurrencyPrefab;
    Dictionary<CurrencyType, ExpeditionCurrencyDefinition> currencyUi = new();

    [SerializeField] Transform ExpeditionShipUpgradesPanel;
    [SerializeField] Transform ExpeditionCrewUpgradesPanel;
    [SerializeField] Transform ExpeditionItensUpgradesPanel;
    [SerializeField] Transform ExpeditionWeaponsControlPanel;
    [SerializeField] ExpeditionUpgradeDefinition UpgradePrefab;
    [SerializeField] ExpeditionWeaponDefinition WeaponPrefab;
    Dictionary<string, ExpeditionUpgradeDefinition> shipUpgradeUI = new();

    [SerializeField] CurrencyView CurrencyIncomePrefab;
    [SerializeField] IngredientView IngredientIncomePrefab;
    [SerializeField] Transform ShipView;
    [SerializeField] CriticalView CriticalStrikePrefab;

    [SerializeField] GameObject TutorialPopUp;
    [SerializeField] TextMeshProUGUI TutorialTitleText;
    [SerializeField] TextMeshProUGUI TutorialInfoText;

    float MessagesTextTimer = 2f;
    float MessagesTimer = 0;
    bool MessageTextShown = false;

    public void Initialize(GameState gameState, PurchaseService purchaseService, ConfigurationsService configs, TutorialService tutorial, TickService tick)
    {               
        GameState = gameState;

        PurchaseService = purchaseService;

        ConfigurationsService = configs;

        TutorialService = tutorial;

        TickService = tick;
    }

    void Start()
    {
        // Inicialização dos Paineis
        HideAllMenus();
        ShipPanel.SetActive(true);
    }

    private void Update()
    {
        if (GameState.ExpeditionState.ExpeditionStatus != GameHelper.ExpeditionStatus.Running)
            return;

        if (MessageTextShown)
        {
            MessagesTimer += Time.deltaTime;

            if (MessagesTimer >= MessagesTextTimer)
            {
                MissionsTextSet();
            }
        }

        if (MessagesTimer >= MessagesTextTimer)
        {
            MessagesTimer = 0;
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

        PathChangeSet();
        LifeTextSet();

        LanguagesDropdownSet();
        GameSpeedText.text = GameState.ActualGameSpeed.ToString();
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
    public void OpenWeaponsMenu()
    {
        HideAllMenus();

        WeaponsPanel.SetActive(true);

        BuildWeapons(ExpeditionWeaponsControlPanel);
    }
    public void OpenSettingMenu()
    {
        HideAllMenus();
        SettingsPanel.SetActive(true);
    }
    public void OpenInfoPanel()
    {
        if (InfoPanelActive)
        {
            LifePanel.SetActive(true);
            InfoPanel.SetActive(false);
            InfoPanelActive = false;
        }
        else
        {
            LifePanel.SetActive(false);
            InfoPanel.SetActive(true);
            InfoPanelActive = true;
        }
    }
    void HideAllMenus()
    {
        ShipPanel.SetActive(false);
        CrewPanel.SetActive(false);
        ItensPanel.SetActive(false);
        WeaponsPanel.SetActive(false);
        SettingsPanel.SetActive(false);
        InfoPanel.SetActive(false);
        LifePanel.SetActive(true);
    }

    // Changers
    private void DayCycleTextSet()
    {
        DaysPastText.text = GameState.ExpeditionState.DayCounter.ToString("N0") + " > " + GameState.ExpeditionState.NextDestination.ToString("N0");

        if (GameState.ExpeditionState.DayCounter > 1)
        {
            if (!GameState.ProgressState.MarcosTut && !GameState.ExpeditionState.IsDay)
            {
                ShowTutorial(GameHelper.Tutorial.MarcosTut);
            }
        }

        if (GameState.ExpeditionState.DayCounter > 1)
        {
            if (!GameState.ProgressState.ExperienceTut && GameState.ExpeditionState.IsDay)
            {
                ShowTutorial(GameHelper.Tutorial.ExperienceTut);
            }
        }
    }
    private void LifeTextSet()
    {
        CurrentLifeText.text =
              Math.Ceiling(GameState.ExpeditionState.Ship.CurrentLife).ToString("N0")
              + " / " +
              Math.Ceiling(GameState.ExpeditionState.Ship.ActualLife).ToString("N0");

        if (GameState.ActualLanguage == GameState.Language.Portugues)
        {
            ShipNameText.text = GameState.ExpeditionState.Ship.NamePT;
            WeaponsText.text = "Armas Equipadas: " + GameState.ExpeditionState.Ship.Weapons.Count.ToString();
            RegenText.text = "Reparos: " + GameState.ExpeditionState.Ship.ActualRepair;
            ArmorText.text = "Armadura: " + GameState.ExpeditionState.Ship.ActualResistence + "%";
            BestTravelText.text = "Destinos Encontrados: " + GameState.ExpeditionState.ReachedDestinations;
            MarcosDayText.text = "Marcos por Dia: " + GameState.ExpeditionState.ActualDayReward;
            ExperienceDayText.text = "Exp. por Dia: " + GameState.ExpeditionState.ActualNightReward;
            DefenseText.text = "Defesa: " + GameState.ExpeditionState.Ship.ActualArmor;
        }
        if (GameState.ActualLanguage == GameState.Language.English)
        {
            ShipNameText.text = GameState.ExpeditionState.Ship.NameEN;
            WeaponsText.text = "Equiped Weapons: " + GameState.ExpeditionState.Ship.Weapons.Count.ToString();
            RegenText.text = "Repair: " + GameState.ExpeditionState.Ship.ActualRepair;
            ArmorText.text = "Armor: " + GameState.ExpeditionState.Ship.ActualResistence + "%";
            BestTravelText.text = "Destinations Found: " + GameState.ExpeditionState.ReachedDestinations;
            MarcosDayText.text = "Marcos per Day: " + GameState.ExpeditionState.ActualDayReward;
            ExperienceDayText.text = "Exp. per Day: " + GameState.ExpeditionState.ActualNightReward;
            DefenseText.text = "Defense: " + GameState.ExpeditionState.Ship.ActualArmor;
        }

    }
    private void PathChangeSet()
    {
        var path = GameState.ExpeditionState.ActualPath;

        var language = GameState.ActualLanguage;

        PathText.text = PathHelper.GetPathTypeName(path.Type, language);

        BiomeText.text = PathHelper.GetEnvironmentName(path.Environment, language);

        ChangeText.text = PathHelper.GetModifierName(path.Modifier, language);

        DayCycleTextSet();

        if (GameState.ExpeditionState.DayCounter > 5)
        {
            if (!GameState.ProgressState.DestinationsTut)
            {
                ShowTutorial(GameHelper.Tutorial.DestinationsTut);
            }
        }
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


    // Missions
    private void MissionsTextSet()
    {
        MessageTextShown = false;
        MessagesText.text = "";
    }
    private void MissionUpdate(MissionRuntime mission)
    {
        if (GameState.ActualLanguage == GameState.Language.Portugues)
        {
            MessagesText.text = "Missão " + mission.NamePT + " Finalizada!";
        }
        else if (GameState.ActualLanguage == GameState.Language.English)
        {
            MessagesText.text = "Mission " + mission.NameEN + " Finished!";
        }

        MessagesTimer = 0f;
        MessageTextShown = true;
    }


    // Reload
    private void WeaponReloadMessage(WeaponInstance weapon)
    {
        if (GameState.ActualLanguage == GameState.Language.Portugues)
        {
            MessagesText.text = "Recarregando " + weapon.NamePT;
        }
        if (GameState.ActualLanguage == GameState.Language.English)
        {
            MessagesText.text = "Recharging " + weapon.NameEN;
        }

        MessagesTimer = 0f;
        MessageTextShown = true;

        if (!GameState.ProgressState.WeaponsTut)
        {
            ShowTutorial(GameHelper.Tutorial.WeaponsTut);
        }
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
    private void BuildWeapons(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }

        foreach (var weapon in GameState.ExpeditionState.Ship.Weapons)
        {
            var obj = Instantiate(WeaponPrefab, parent);

            var ui = obj.GetComponent<ExpeditionWeaponDefinition>();

            ui.Setup(weapon, GameState);
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


    // Effects View
    private void CurrencyIncome(CurrencyInstance currency, double amount)
    {
        var income = Instantiate(CurrencyIncomePrefab);

        income.Setup(
            currency,
            amount,
            ShipView.position
        );
    }
    private void IngredientIncome(IngredientInstance currency, double amount)
    {
        var income = Instantiate(IngredientIncomePrefab);

        income.Setup(
            currency,
            amount,
            ShipView
        );
    }
    private void EnemyIncome(EnemyRuntime enemy, Vector3 position)
    {
        var income = Instantiate(CurrencyIncomePrefab, position, Quaternion.identity);

        income.Setup(
            GameState.DataState.currencies[CurrencyType.Experience],
            enemy.Experience,
            position
        );

        if (!GameState.ProgressState.ClickTut)
        {
            ShowTutorial(GameHelper.Tutorial.ClickTut);
        }
    }
    private void CriticalStrike(EnemyRuntime enemy, Vector3 position)
    {
        var critico = Instantiate(CriticalStrikePrefab, position, Quaternion.identity);

        critico.Setup(
            position,
            GameState
        );
    }


    // Tutorial
    private void ShowTutorial(GameHelper.Tutorial type)
    {
        TickService.Pause();

        var text = TutorialService.SetText(type);

        TutorialTitleText.text = text.Item1;
        TutorialInfoText.text = text.Item2;

        TutorialService.SetText(type);
        TutorialPopUp.SetActive(true);
    }
    public void CloseTutorial()
    {
        TutorialPopUp.SetActive(false);
        TickService.Resume();
    }
    private void NewEnemy(EnemyInstance enemy)
    {
        if (!GameState.ProgressState.KnowledgeTut)
        {
            ShowTutorial(GameHelper.Tutorial.KnowledgeTut);
        }
    }


    // Eventos
    void OnEnable()
    {
        ExpeditionEvents.OnShipAtributeChange += RefreshShipUi;
        ExpeditionEvents.CriticalDamage += CriticalStrike;

        GameEvents.OnUpgradeBought += RefreshUpgradeUi;
        GameEvents.OnCurrencyChange += RefreshCurrencyUi;
        GameEvents.OnCanBuyChange += RefreshCurrencyUi;
        GameEvents.OnMissionUpdate += MissionUpdate;

        GameEvents.OnLanguageChange += RefreshLanguageUi;

        ExpeditionEvents.OnExpeditionStart += GameStart;
        ExpeditionEvents.OnDayFinish += DayCycleTextSet;
        ExpeditionEvents.OnNightFinish += DayCycleTextSet;

        ExpeditionEvents.OnDestinationArrival += PathChangeSet;
        ExpeditionEvents.OnPathSet += PathChangeSet;

        ExpeditionEvents.OnRechargeStart += WeaponReloadMessage;

        ExpeditionEvents.CurrencyIncome += CurrencyIncome;
        ExpeditionEvents.IngredientIncome += IngredientIncome;
        ExpeditionEvents.OnEnemyDeath += EnemyIncome;

        GameEvents.NewEnemySeen += NewEnemy;
    }

    void OnDisable()
    {
        ExpeditionEvents.OnShipAtributeChange -= RefreshShipUi;
        ExpeditionEvents.CriticalDamage -= CriticalStrike;

        GameEvents.OnUpgradeBought -= RefreshUpgradeUi;
        GameEvents.OnCurrencyChange -= RefreshCurrencyUi;
        GameEvents.OnCanBuyChange -= RefreshCurrencyUi;
        GameEvents.OnMissionUpdate -= MissionUpdate;

        GameEvents.OnLanguageChange -= RefreshLanguageUi;

        ExpeditionEvents.OnExpeditionStart -= GameStart;
        ExpeditionEvents.OnDayFinish -= DayCycleTextSet;
        ExpeditionEvents.OnNightFinish -= DayCycleTextSet;

        ExpeditionEvents.OnDestinationArrival -= PathChangeSet;
        ExpeditionEvents.OnPathSet -= PathChangeSet;

        ExpeditionEvents.OnRechargeStart -= WeaponReloadMessage;

        ExpeditionEvents.CurrencyIncome -= CurrencyIncome;
        ExpeditionEvents.IngredientIncome -= IngredientIncome;
        ExpeditionEvents.OnEnemyDeath -= EnemyIncome;

        GameEvents.NewEnemySeen -= NewEnemy;
    }

    void GameStart()
    {
        CurrenciesBuild();
        BuildShipUpgrades(ExpeditionShipUpgradesPanel);
        BuildCrewUpgrades(ExpeditionCrewUpgradesPanel);
        BuildItensUpgrades(ExpeditionItensUpgradesPanel);
        BuildWeapons(ExpeditionWeaponsControlPanel);

        foreach (var upgrade in GameState.DataState.upgrades)
        {
            if (upgrade.Value.UnlockStatus == UnlockHelper.UnlockStatus.Unlocked)
            {
                RefreshUpgradeUi(upgrade.Value);
            }
        }

        if (!GameState.ProgressState.ExpeditionTut)
        {
            ShowTutorial(GameHelper.Tutorial.ExpeditionTut);
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

        if (!GameState.ProgressState.ShipTut)
        {
            if (GameState.ExpeditionState.Ship.CurrentLife < GameState.ExpeditionState.Ship.ActualLife)
            {
                ShowTutorial(GameHelper.Tutorial.ShipTut);
            }
        }
    }

    void RefreshUpgradeUi(UpgradeInstance upgrade)
    {
        UpgradeSet(upgrade);

        if (!GameState.ProgressState.UpgradesTut)
        {
            ShowTutorial(GameHelper.Tutorial.UpgradesTut);
        }
    }

    private void RefreshLanguageUi()
    {
        UpgradesSet(CurrencyType.Experience);
    }
}
