using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    private DecisionsPopUpDesigner Panel;

    private List<DestinationInstance> DestinationOptions;

    public void Initialize(ExpeditionState expedition, ShipState ship, GameState game, DecisionsPopUpDesigner panel, TickService tick, DataState db, ExpeditionService expeditionService)
    {
        TickService = tick;
        ExpeditionState = expedition;
        ShipState = ship;
        GameState = game;
        Panel = panel;
        DataState = db;
        ExpeditionService = expeditionService;

        DestinationOptions = new List<DestinationInstance>();

        Debug.Log("DecisionsService On");
        Debug.Log($"Primeira Expedição: {GameState.FirstExpedition}");

        if (GameState.FirstExpedition)
        {
            DestinationOptions.Clear();
            DestinationOptions.Add(DataState.destinations.GetValueOrDefault("d101"));
            FirstDecision(DestinationOptions);

            // Mudar para um SaveService
            GameState.FirstExpedition = false;

            PlayerPrefs.SetInt("HasInitialized", 1);
            PlayerPrefs.Save();

            Debug.Log($"Primeira Expedição: {GameState.FirstExpedition}");
        }
    }

    public void FirstDecision(List<DestinationInstance> options)
    {
        Debug.Log("Primeira Escolha de Destination Iniciada");

        TickService.Pause();

        Panel.ShowDestinations(options, FirstSelection);
    }

    private void FirstSelection(DestinationInstance selected)
    {
        Debug.Log($"Primeiro Destino: {selected.Name}");

        if (DataState == null)
        {
            Debug.LogError("DataState não encontrada na Decision!");
            return;
        }

        ExpeditionState.OldDestination = DataState.destinations.GetValueOrDefault("d000");
        ExpeditionState.CurrentPath = DataState.paths.GetValueOrDefault("p000101");
        ExpeditionState.NewDestination = selected;

        ExpeditionState.DestinationArrival = CalculateDays(ExpeditionState.CurrentPath, ShipState.Ship);

        Panel.Hide();

        ExpeditionState.ExpeditionStatus = ExpeditionStatus.Running;

        TickService.Resume();

        ExpeditionService.NewDestinationChose();
    }

    private void DestinationSelection(DestinationInstance selected)
    {
        Debug.Log($"Destino escolhido: {selected.Name}");

        if (DataState == null)
        {
            Debug.LogError("DataState não encontrada na Decision!");
            return;
        }

        ExpeditionState.OldDestination = ExpeditionState.NewDestination;
        // trocar para PathService e definir rota entre os dois destinos
        ExpeditionState.CurrentPath = DataState.paths.GetValueOrDefault("p000101");
        ExpeditionState.NewDestination = selected;

        ExpeditionState.DestinationArrival = CalculateDays(ExpeditionState.CurrentPath, ShipState.Ship);

        Panel.Hide();

        TickService.Resume();
    }

    private int CalculateDays(PathInstance Path, ShipInstance Ship)
    {
        int RealDistance = (int)Math.Ceiling(Path.Distance / Ship.Model.Speed);

        return RealDistance;
    }
}
