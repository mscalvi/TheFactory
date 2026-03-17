using System.Collections.Generic;
using UnityEngine;

public class ExpeditionService : MonoBehaviour
{
    // Controlador de Inimigos
    public float BaseSpawnDistance = 20f;
    public int BaseSpawnTimer = 10;

    public List<EnemyInstance> ActiveEnemies = new();


    // Controlador de Dias
    public int BasePhaseCycleTime = 75;
    public bool IsDay { get; set; } = true;

    public int DayCounter = 1;
    public int DestinationArrival;
}