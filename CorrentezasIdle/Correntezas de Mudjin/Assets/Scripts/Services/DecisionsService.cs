using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.SearchService;
using UnityEngine;
using static GameHelper;

public class DecisionsService : MonoBehaviour
{
    private TickService TickService;
    private ExpeditionService ExpeditionService;

    private GameState GameState;
    private ShipState ShipState;
    private DataState DataState;
    private ExpeditionState ExpeditionState;

    private DecisionsPopUpDesigner DecisionPanel;
    private FinalPopUpDesigner FinalPanel;

    private List<DestinationInstance> DestinationOptions;

    public void Initialize(ExpeditionState expedition, ShipState ship, GameState game, DecisionsPopUpDesigner panel, TickService tick, DataState db, ExpeditionService expeditionService, FinalPopUpDesigner finalpanel)
    {
        TickService = tick;
        ExpeditionService = expeditionService;

        ExpeditionState = expedition;
        ShipState = ship;
        GameState = game;
        DataState = db;

        DecisionPanel = panel;
        FinalPanel = finalpanel;

        DestinationOptions = new List<DestinationInstance>();

        if (GameState.FirstExpedition)
        {
            ExpeditionState.FirstExpedition = true;

            DestinationOptions.Clear();
            DestinationOptions.Add(DataState.destinations.GetValueOrDefault("d101"));
            FirstDecision(DestinationOptions);

            PlayerPrefs.SetInt("HasInitialized", 1);
            PlayerPrefs.Save();
        }
    }

    public void FirstDecision(List<DestinationInstance> options)
    {
        TickService.Pause();

        // Removido, deixar entrada automática
        //DecisionPanel.ShowDestinations(options, FirstSelection);

        FirstSelection(DataState.destinations.GetValueOrDefault("d101"));
    }

    private void FirstSelection(DestinationInstance selected)
    {
        ExpeditionState.OldDestination = DataState.destinations.GetValueOrDefault("d000");
        ExpeditionState.CurrentPath = DataState.paths.GetValueOrDefault("p000101");
        ExpeditionState.NewDestination = selected;

        ExpeditionState.DestinationArrival = CalculateDays(ExpeditionState.CurrentPath, ShipState.Ship);

        DecisionPanel.Hide();

        ExpeditionState.ExpeditionStatus = ExpeditionStatus.Running;

        TickService.Resume();

        ExpeditionService.NewDestinationChose();
    }

    public void DestinationDecision(List<DestinationInstance> options)
    {
        TickService.Pause();

        DecisionPanel.ShowDestinations(options, DestinationSelection);
    }

    private void DestinationSelection(DestinationInstance selected)
    {
        ExpeditionState.OldDestination = ExpeditionState.NewDestination;
        // trocar para PathService e definir rota entre os dois destinos
        ExpeditionState.CurrentPath = DataState.paths.GetValueOrDefault("p000101");
        ExpeditionState.NewDestination = selected;

        ExpeditionState.DestinationArrival = CalculateDays(ExpeditionState.CurrentPath, ShipState.Ship);

        DecisionPanel.Hide();

        TickService.Resume();
    }

    public void LastDecision(bool victory)
    {
        TickService.Pause();

        FinalPanel.ShowResults(victory, ExpeditionState, LastSelecion);
    }

    private void LastSelecion(bool victory)
    {
        RunEvents.OnFinalPopUpClose?.Invoke();
    }

    private int CalculateDays(PathInstance Path, ShipInstance Ship)
    {
        int RealDistance = (int)Math.Ceiling(Path.Distance / Ship.BaseSpeed);

        return RealDistance;
    }

    void OnEnable()
    {
        RunEvents.OnDestinationArrival += ChooseDestination;
    }

    void OnDisable()
    {
        RunEvents.OnDestinationArrival += ChooseDestination;
    }

    void ChooseDestination()
    {
        // OptionsService.Destinations();
        // DestinationDecision();
        TickService.Pause();
    }
}
