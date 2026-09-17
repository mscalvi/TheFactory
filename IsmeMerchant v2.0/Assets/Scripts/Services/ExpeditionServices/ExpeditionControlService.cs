using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameHelper;

public class ExpeditionControlService : MonoBehaviour, ITickable
{
    private GameState GameState;
    private ExpeditionState ExpeditionState;
    
    private SaveService SaveService;
    private TickService TickService;

    public void Initialize(GameState game, TickService Tick, SaveService save)
    {
        GameState = game;

        ExpeditionState = GameState.ExpeditionState;

        SaveService = save;

        TickService = Tick;

        TickService.Subscribe(this);
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

    private void Die()
    {
        ExpeditionState.Ship.CurrentLife = 0;

        Death();
    }

    public void Death()
    {
        ExpeditionState.ExpeditionStatus = ExpeditionStatus.GameOver;

        GameState.DataState.currencies[CurrencyHelper.CurrencyType.Experience].Amount = 0;

        ExpeditionEvents.OnExpeditionEnd?.Invoke();
        SaveService.Save();
    }

    public void LoadLandingPage()
    {
        ExpeditionState.ExpeditionStatus = ExpeditionStatus.Finished;
        SceneManager.LoadScene("LandingScene");
    }

    // Events

    void OnEnable()
    {
        ExpeditionEvents.OnFinalPopUpClose += LoadLandingPage;

        GameEvents.LifeTest += Die;
    }

    void OnDisable()
    {
        ExpeditionEvents.OnFinalPopUpClose -= LoadLandingPage;

        GameEvents.LifeTest -= Die;
    }
}