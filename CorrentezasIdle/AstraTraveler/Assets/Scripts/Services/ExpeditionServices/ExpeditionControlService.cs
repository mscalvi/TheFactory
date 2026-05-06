using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameHelper;

public class ExpeditionControlService : MonoBehaviour, ITickable
{
    private TickService TickService;
    private GameState GameState;
    private ExpeditionState ExpeditionState;

    public void Initialize(GameState game, TickService Tick)
    {
        GameState = game;

        ExpeditionState = GameState.ExpeditionState;

        TickService = Tick;

        TickService.Subscribe(this);

        LoadExpedition(GameState);
        LoadShip(GameState);
    }

    void OnDestroy()
    {
        TickService?.Unsubscribe(this);
    }

    public void OnTick(float dt)
    {
        if (ExpeditionState.Ship.CurrentLife <= 0)
        {
            Death();
        }
    }

    void LoadExpedition(GameState Game)
    {
        Game.ExpeditionState.ActiveEnemies.Clear();

        Game.ExpeditionState.DayCounter = Game.ExpeditionState.StartDay;

        Game.ExpeditionState.ActualSpawnChance = Game.ExpeditionState.BaseSpawnChance;
        Game.ExpeditionState.ActualTicksPerSpawn = Game.ExpeditionState.BaseTicksPerSpawn;
        Game.ExpeditionState.ActualSpawnBudget = Game.ExpeditionState.BaseSpawnBudget;
        Game.ExpeditionState.ActualSpawnBudgetGrowth = Game.ExpeditionState.BaseSpawnBudgetGrowth;
        Game.ExpeditionState.ActualBossThreshold = Game.ExpeditionState.BaseBossThreshold;
        Game.ExpeditionState.ActualDayReward = Game.ExpeditionState.BaseDayReward;
        Game.ExpeditionState.ActualNightReward = Game.ExpeditionState.BaseNightReward;
        Game.ExpeditionState.ActualMaxMarkedEnemies = Game.ExpeditionState.BaseMaxMarkedEnemies;
        Game.ExpeditionState.ActualMaxMarkedLoot = Game.ExpeditionState.BaseMaxMarkedLoot;
        Game.ExpeditionState.ActualNextLootChance = Game.ExpeditionState.BaseNextLootChance;
        Game.ExpeditionState.ActualNextLootDecay = Game.ExpeditionState.BaseNextLootDecay;
    }

    void LoadShip(GameState Game)
    {
        Game.ExpeditionState.Ship.CurrentArmor = Game.ExpeditionState.Ship.BaseArmor;
        Game.ExpeditionState.Ship.CurrentLife = Game.ExpeditionState.Ship.BaseLife;
        Game.ExpeditionState.Ship.CurrentSpeed = Game.ExpeditionState.Ship.BaseSpeed;

        foreach (var weapon in Game.ExpeditionState.Ship.Weapons)
        {
            weapon.ActualDamage = weapon.BaseDamage;
            weapon.ActualAttackSpeed = weapon.BaseAttackSpeed;
            weapon.ActualRange = weapon.BaseRange;
            weapon.ActualCriticalDamage = weapon.BaseCriticalDamage;
            weapon.ActualPrecision = weapon.BasePrecision;
        }
    }

    public void Death()
    {
        ExpeditionState.ExpeditionStatus = ExpeditionStatus.GameOver;

        GameState.DataState.currencies[CurrencyHelper.CurrencyType.Experience].Amount = 0;

        ExpeditionEvents.OnExpeditionEnd?.Invoke();
    }

    public void LoadLandingPage()
    {
        SceneManager.LoadScene("LandingScene");
    }

    // Events

    void OnEnable()
    {
        ExpeditionEvents.OnFinalPopUpClose += LoadLandingPage;
    }

    void OnDisable()
    {
        ExpeditionEvents.OnFinalPopUpClose -= LoadLandingPage;
    }
}