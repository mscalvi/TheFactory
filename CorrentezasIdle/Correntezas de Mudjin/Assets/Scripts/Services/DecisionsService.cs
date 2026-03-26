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
    private ExpeditionState ExpeditionState;
    private DecisionsPopUpDesigner Panel;
    private GameDatabase GameDatabase;

    private List<DestinationModel> DestinationOptions;

    public void Initialize(ExpeditionState expedition, ShipState ship, GameState game, DecisionsPopUpDesigner panel, TickService tick, GameDatabase db, ExpeditionService expeditionService)
    {
        TickService = tick;
        ExpeditionState = expedition;
        ShipState = ship;
        GameState = game;
        Panel = panel;
        GameDatabase = db;
        ExpeditionService = expeditionService;

        DestinationOptions = new List<DestinationModel>();

        Debug.Log("DecisionsService On");
        Debug.Log($"Primeira Expedição: {GameState.FirstExpedition}");

        if (GameState.FirstExpedition)
        {
            DestinationOptions.Clear();
            DestinationOptions.Add(GameDatabase.destinations[1]);
            FirstDecision(DestinationOptions);

            // Mudar para um SaveService
            GameState.FirstExpedition = false;

            PlayerPrefs.SetInt("HasInitialized", 1);
            PlayerPrefs.Save();

            Debug.Log($"Primeira Expedição: {GameState.FirstExpedition}");
        }
    }

    public void FirstDecision(List<DestinationModel> options)
    {
        Debug.Log("Primeira Escolha de Destination Iniciada");

        TickService.Pause();

        Panel.ShowDestinations(options, FirstSelection);
    }

    private void FirstSelection(DestinationModel selected)
    {
        Debug.Log($"Primeiro Destino: {selected.Name}");

        if (GameDatabase == null)
        {
            Debug.LogError("DataBase não encontrada na Decision!");
            return;
        }

        ExpeditionState.OldDestination = GameDatabase.destinations[0];
        ExpeditionState.CurrentPath = GameDatabase.paths[0];
        ExpeditionState.NewDestination = selected;

        ExpeditionState.DestinationArrival = CalculateDays(ExpeditionState.CurrentPath, ShipState.Ship);

        Panel.Hide();

        ExpeditionState.ExpeditionStatus = ExpeditionStatus.Running;

        TickService.Resume();

        ExpeditionService.NewDestinationChose();
    }

    private void DestinationSelection(DestinationModel selected)
    {
        Debug.Log($"Destino escolhido: {selected.Name}");

        if (GameDatabase == null)
        {
            Debug.LogError("DataBase não encontrada na Decision!");
            return;
        }

        ExpeditionState.OldDestination = ExpeditionState.NewDestination;
        // trocar para PathService e definir rota entre os dois destinos
        ExpeditionState.CurrentPath = GameDatabase.paths[0];
        ExpeditionState.NewDestination = selected;

        ExpeditionState.DestinationArrival = CalculateDays(ExpeditionState.CurrentPath, ShipState.Ship);

        Panel.Hide();

        TickService.Resume();
    }

    private int CalculateDays(PathModel Path, ShipInstance Ship)
    {
        int RealDistance = (int)Math.Ceiling(Path.Distance / Ship.Model.Speed);

        return RealDistance;
    }
}
