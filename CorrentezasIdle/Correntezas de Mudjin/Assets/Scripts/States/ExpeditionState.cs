using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpeditionState
{
    public ExpeditionConfiguration CurrentExpedition;

    // Day/Night Service
    public int BaseTicksPerPhase = 75;
    public bool IsDay { get; set; } = true;

    public int DayCounter = 1;
    public int DestinationArrival = 0;

    // Controlador de Inimigos
    public double BaseSpawnDistance = 1.5;
    public double BaseSpawnChance = 1;
    public int BaseTicksPerSpawn = 15;

    public List<EnemyInstance> ActiveEnemies = new();

    // Controlador do Navio
    public ShipModel currentShip;
    public ShipInstance Ship;
}
