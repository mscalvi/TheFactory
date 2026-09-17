using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpeditionManager : MonoBehaviour
{
    [SerializeField] MapService MapService;
    [SerializeField] PathService PathService;

    [SerializeField] EnemyService EnemyService;

    [SerializeField] TowerService TowerService;

    void Start()
    {
        MapData mapData = new MapData();

        MapService.Initialize(mapData);
        PathService.Initialize(MapService);

        EnemyService.Initialize(PathService);

        TowerService.Initialize(MapService);
    }
}
