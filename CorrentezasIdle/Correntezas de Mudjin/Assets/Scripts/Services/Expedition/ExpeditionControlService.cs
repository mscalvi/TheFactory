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
        Game.ExpeditionState = new ExpeditionState();
        Game.ExpeditionState.ActiveEnemies.Clear();
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
        Game.ShipState.Ship.CurrentLife = Game.ShipState.Ship.BaseLife;
        Game.ShipState.Ship.CurrentSpeed = Game.ShipState.Ship.BaseSpeed;
    }

    public void Death()
    {
        Debug.Log("Expedition ExpeditionService - Game Over!");

        ExpeditionState.ExpeditionStatus = ExpeditionStatus.GameOver;

        Debug.Log($"Expedition EndExpeditionService - Zerando Experience");

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