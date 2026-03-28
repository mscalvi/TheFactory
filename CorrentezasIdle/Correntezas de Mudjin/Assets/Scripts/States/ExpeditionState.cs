using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpeditionState
{
    // public ExpeditionConfiguration CurrentExpedition;

    // Day/Night Service
    public int BaseTicksPerPhase = 75;
    public bool IsDay { get; set; } = true;
    public int DestinationDayCounter = 1;
    public int DayCounter = 1;

    // Destination
    public DestinationInstance OldDestination;
    public DestinationInstance NewDestination;
    public PathInstance CurrentPath;
    public int DestinationArrival = 0;
    public GameHelper.ExpeditionStatus ExpeditionStatus;

    // Controlador de Inimigos
    public double BaseSpawnDistance = 1.5;
    public double BaseSpawnChance = 1;
    public int BaseTicksPerSpawn = 15;

    public List<EnemyInstance> ActiveEnemies = new();

    // Currency
    public Dictionary<CurrencyHelper.CurrencyType, CurrencyInstance> ExpeditionCurrency;
    public double BaseDayReward = 1;
    public double BaseNightReward = 1;
}
