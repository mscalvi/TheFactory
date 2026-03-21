using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameHelper;

public class ExpeditionController : MonoBehaviour
{
    public void Awake()
    {
        RunController runControler = BuildConfig();

        GameController.Instance.CurrentRun = runControler;
    }

    RunController BuildConfig()
    {
        var config = new RunController();

        //// Aqui você preenche com base na UI
        //config.Ship = selectedShip;

        //config.Rooms = new List<RoomConfig>();

        //foreach (var roomUI in roomUIs)
        //{
        //    config.Rooms.Add(new RoomConfig
        //    {
        //        Weapon = roomUI.SelectedWeapon,
        //        TargetType = roomUI.SelectedTargetType
        //    });
        //}

        return config;
    }
}