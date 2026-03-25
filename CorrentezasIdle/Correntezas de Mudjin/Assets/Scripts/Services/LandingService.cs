using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingService : MonoBehaviour
{
    private GameDatabase Database;
    private GameState Game;
    private ShipInitialConfiguration ShipConfiguration;

    public void Initialize(GameState gameState, GameDatabase db)
    {
        Game = gameState;
        Database = db;

        if (Game.ShipInitialConfiguration == null)
        {
            Debug.Log("Nenhum Navio Carregado.");
            CreateFirstExpedition();
        } else
        {
            // Carregar a configuração anterior
        }

        Debug.Log("LandingService Finalizado");
    }

    public void CreateFirstExpedition()
    {
        ShipConfiguration = new ShipInitialConfiguration();
        ShipConfiguration.WeaponRooms = new List<WeaponRoomInitialConfiguration>();

        var ship = Database.ships[0];
        ShipConfiguration.Ship = ship;

        // Weapon Rooms
        foreach (var room in ship.WeaponRooms)
        {
            var config = new WeaponRoomInitialConfiguration();

            config.RoomId = room.WeaponRoomModel.Id;
            config.Tripulation = Database.tripulation[0];
            config.Weapon = Database.weapons[0];
            config.Ammo = Database.ammos[0];

            ShipConfiguration.WeaponRooms.Add(config);
        }

        Game.ShipState = new ShipState();
        Game.ShipInitialConfiguration = ShipConfiguration;

        Game.FirstExpedition = false;

        PlayerPrefs.SetInt("HasInitialized", 1);
        PlayerPrefs.Save();

        Debug.Log($"Navio: {Game.ShipInitialConfiguration.Ship.Name} Carregado.");
    }
}
