using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameHelper;

public class ExpeditionControlService : MonoBehaviour, ITickable
{
    private TickService TickService;
    private ShipState ShipState;
    private GameState GameState;
    private ExpeditionState ExpeditionState;

    public void Initialize(GameState game, TickService Tick)
    {
        GameState = game;

        ShipState = GameState.ShipState;

        ExpeditionState = GameState.ExpeditionState;

        TickService = Tick;

        TickService.Subscribe(this);

        LoadExpedition(GameState);
        LoadDestination(GameState);
        LoadShip(GameState);
    }

    void OnDestroy()
    {
        TickService?.Unsubscribe(this);
    }

    public void OnTick(float dt)
    {
        if (ShipState.Ship.CurrentLife <= 0)
        {
            Death();
        }
    }

    void LoadExpedition(GameState Game)
    {
        Game.ExpeditionState.ActiveEnemies.Clear();

        Game.ExpeditionState.DayCounter = Game.ExpeditionState.StartDay;
        Game.ExpeditionState.DestinationDayCounter = Game.ExpeditionState.StartDay;

        Game.ExpeditionState.ActualSpawnDistance = Game.ExpeditionState.BaseSpawnDistance;
        Game.ExpeditionState.ActualSpawnChance = Game.ExpeditionState.BaseSpawnChance;
        Game.ExpeditionState.ActualTicksPerSpawn = Game.ExpeditionState.BaseTicksPerSpawn;
        Game.ExpeditionState.ActualSpawnBudget = Game.ExpeditionState.BaseSpawnBudget;
        Game.ExpeditionState.ActualSpawnBudgetGrowth = Game.ExpeditionState.BaseSpawnBudgetGrowth;
        Game.ExpeditionState.ActualBossThreshold = Game.ExpeditionState.BaseBossThreshold;
        Game.ExpeditionState.ActualEnemySpawnStage = Game.ExpeditionState.BaseEnemySpawnStage;
        Game.ExpeditionState.ActualDayReward = Game.ExpeditionState.BaseDayReward;
        Game.ExpeditionState.ActualNightReward = Game.ExpeditionState.BaseNightReward;
        Game.ExpeditionState.ActualMaxMarkedEnemies = Game.ExpeditionState.BaseMaxMarkedEnemies;
        Game.ExpeditionState.ActualMaxMarkedLoot = Game.ExpeditionState.BaseMaxMarkedLoot;
        Game.ExpeditionState.ActualNextLootChance = Game.ExpeditionState.BaseNextLootChance;
        Game.ExpeditionState.ActualNextLootDecay = Game.ExpeditionState.BaseNextLootDecay;
    }

    void LoadDestination(GameState Game)
    {
        if (GameState.ProgressState.m000)
        {
            Game.ExpeditionState.ActualDestination = Game.CompanyState.CurrentBase;
        }
    }

    void LoadShip(GameState Game)
    {
        Game.ShipState.Ship.CurrentArmor = Game.ShipState.Ship.BaseArmor;
        Game.ShipState.Ship.CurrentLife = (int)Game.ShipState.Ship.BaseLife;
        Game.ShipState.Ship.CurrentSpeed = Game.ShipState.Ship.BaseSpeed;

        foreach (var room in Game.ShipState.Ship.WeaponsRooms)
        {
            room.Weapon.ActualDamage = room.Weapon.BaseDamage;
            room.Weapon.ActualAttackSpeed = room.Weapon.BaseAttackSpeed;
            room.Weapon.ActualRange = room.Weapon.BaseRange;
            room.Weapon.ActualCriticalDamage = room.Weapon.BaseCriticalDamage;
            room.Weapon.ActualPrecision = room.Weapon.BasePrecision;
        }
    }

    public void Death()
    {
        ExpeditionState.ExpeditionStatus = ExpeditionStatus.GameOver;

        GameState.CompanyState.CompanyCurrency[CurrencyHelper.CurrencyType.Experience].Amount = 0;


        ExpeditionEvents.OnShipDeath?.Invoke(false);
    }

    public void NewDestinationChose()
    {
        ExpeditionState.DestinationDayCounter = 1;
    }

    public void LoadLandingPage()
    {
        SceneManager.LoadScene("LandingScene");
    }

    // Events

    void OnEnable()
    {
        ExpeditionEvents.OnFinalPopUpClose += LoadLandingPage;
        ExpeditionEvents.OnDestinationChose += NewDestinationChose;
    }

    void OnDisable()
    {
        ExpeditionEvents.OnFinalPopUpClose -= LoadLandingPage;
        ExpeditionEvents.OnDestinationChose -= NewDestinationChose;
    }
}