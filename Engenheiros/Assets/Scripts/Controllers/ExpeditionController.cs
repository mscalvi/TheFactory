using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpeditionController : MonoBehaviour
{
    [SerializeField] GridView GridView;
    [SerializeField] TileView TileView;

    [SerializeField] GridService GridService;

    private void Awake()
    {
        var Game = GameController.Instance.GameState;

        if (Game == null)
        {
            Debug.LogError("ExpeditionController - Game NULL!");
            return;
        }

        var Expedition = GameController.Instance.GameState.Expedition;
        if (Expedition == null)
        {
            Debug.Log("ExpeditionController - ExpeditionState NULL!");
            return;
        }

        //Expedition.ExpeditionStatus = ExpeditionStatus.Loading;

        GridService.Initialize(Game, GridView, TileView);
    }
}
