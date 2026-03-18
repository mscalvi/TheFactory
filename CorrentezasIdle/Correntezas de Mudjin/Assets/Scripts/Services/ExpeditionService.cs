using System.Collections.Generic;
using UnityEngine;
using static GameHelper;

public class ExpeditionService : MonoBehaviour
{
    private void Start()
    {
        Ship = new ShipInstance(currentShip);

        StartExpedition();
    }


    // Controlador do Estado de Jogo
    public GameState State { get; private set; } = GameState.Stopped;

    public void StartExpedition()
    {
        State = GameState.Running;
    }

    public void EndExpedition()
    {
        Debug.Log("Game Over!");
        State = GameState.GameOver;
    }


    // Controlador de Inimigos
    public double BaseSpawnDistance = 1.5;
    public double BaseSpawnChance = 1;
    public int BaseSpawnTimer = 5;

    public List<EnemyInstance> ActiveEnemies = new();


    // Controlador de Dias
    public int BasePhaseCycleTime = 75;
    public bool IsDay { get; set; } = true;
    
    public int DayCounter = 1;
    public int DestinationArrival;

    // Controlador do Navio
    public ShipModel currentShip;
    public ShipInstance Ship;
}