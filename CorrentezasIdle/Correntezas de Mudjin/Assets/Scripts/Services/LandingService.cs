using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingService : MonoBehaviour
{
    public ExpeditionConfiguration ExpeditionConfiguration;
    public GameDatabase Database;
    public GameState GameState;
    public ExpeditionState ExpeditionState;

    public void CreateFirstExpedition()
    {
        if (ExpeditionConfiguration == null)
            ExpeditionConfiguration = new ExpeditionConfiguration();

        if (ExpeditionConfiguration.Rooms == null)
            ExpeditionConfiguration.Rooms = new List<RoomConfiguration>();

        if (ExpeditionState == null)
            ExpeditionState = new ExpeditionState();

        var ship = Database.ships[0];
        ExpeditionConfiguration.Ship = ship;

        // Weapon Rooms
        foreach (var room in ship.WeaponRooms)
        {
            var config = new RoomConfiguration();

            config.RoomId = room.RoomModel.Id;
            config.Tripulation = Database.tripulation[0];
            config.Weapon = Database.weapons[0];
            config.Ammo = Database.ammos[0];

            ExpeditionConfiguration.Rooms.Add(config);
        }

        GameState.Expedition = new ExpeditionState();
        GameState.Expedition.CurrentExpedition = ExpeditionConfiguration;

        GameState.FirstExpedition = false;
    }
}
