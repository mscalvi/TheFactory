using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCreationService : MonoBehaviour
{
    private GameState GameState;
    private GameDatabase DataBase;

    public void Initialize(GameState gs, GameDatabase db)
    {
        GameState = gs;
        DataBase = db;

        Debug.Log("GameCreationService Iniciado");

        CreateDataState(db);
        BuildShips();
    }

    private void CreateDataState(GameDatabase DataBase)
    {
        GameState.DataState = new DataState();

        var ships = new Dictionary<string, ShipInstance>();
        var tripulation = new Dictionary<string, TripulationInstance>();
        var weapons = new Dictionary<string, WeaponInstance>();
        var ammos = new Dictionary<string, AmmoInstance>();
        var weaponsRooms = new Dictionary<string, WeaponRoomInstance>();
        //OtherRoomInstance[] otherRooms;
        var enemies = new Dictionary<string, EnemyInstance>();        
        var destinations = new Dictionary<string, DestinationInstance>();
        var paths = new Dictionary<string, PathInstance>();
        var currencies = new Dictionary<string, CurrencyInstance>();

        foreach (var ship in DataBase.ships)
        {
            var instance = new ShipInstance(ship);
            ships.Add(ship.Id, instance);
        }

        foreach (var trip in DataBase.tripulation)
        {
            var instance = new TripulationInstance(trip);
            tripulation.Add(trip.Id, instance);
        }

        foreach (var weapon in DataBase.weapons)
        {
            var instance = new WeaponInstance(weapon);
            weapons.Add(weapon.Id, instance);
        }

        foreach (var ammo in DataBase.ammos)
        {
            var instance = new AmmoInstance(ammo);
            ammos.Add(ammo.Id, instance);
        }

        foreach (var weaponRoom in DataBase.weaponsRooms)
        {
            var instance = new WeaponRoomInstance(weaponRoom);
            weaponsRooms.Add(weaponRoom.Id, instance);
        }

        foreach (var enemy in DataBase.enemies)
        {
            var instance = new EnemyInstance(enemy);
            enemies.Add(enemy.Id, instance);
        }

        foreach (var path in DataBase.paths)
        {
            var instance = new PathInstance(path);
            paths.Add(path.Id, instance);
        }

        foreach (var dest in DataBase.destinations)
        {
            var instance = new DestinationInstance(dest);
            destinations.Add(dest.Id, instance);
        }

        foreach (var currency in DataBase.currency)
        {
            var instance = new CurrencyInstance(currency);
            currencies.Add(currency.Id, instance);
        }

        GameState.DataState.ships = ships;
        GameState.DataState.tripulations = tripulation;
        GameState.DataState.weapons = weapons;
        GameState.DataState.ammos = ammos;
        GameState.DataState.weaponsRooms = weaponsRooms;
        GameState.DataState.enemies = enemies;
        GameState.DataState.destinations = destinations;
        GameState.DataState.paths = paths;
        GameState.DataState.currencies = currencies;
    }

    private void BuildShips()
    {
        foreach (var ship in GameState.DataState.ships)
        {
            ship.Value.WeaponsRooms = new List<WeaponRoomInstance>();

            for (int i = 0; i < ship.Value.WeaponRoomSlots.Count; i++)
            {
                string subId = ship.Value.Id + i.ToString();
                var instance = new WeaponRoomInstance(ship.Value.WeaponRoomSlots[i].WeaponRoomModel, subId);

                ship.Value.WeaponsRooms.Add(instance);
            }

            //for (int i = 0; i <= ship.Value.OtherRoomSlots.Count; i++)
            //{
            //    Debug.Log($"Construindo Room {ship.Value.OtherRoomSlots[i].OtherRoomModel.Name} no {ship.Key}");

            //    string subId = ship.Value.Id + i.ToString();
            //    var instance = new OtherRoomInstance(ship.Value.OtherRoomSlots[i].OtherRoomModel, subId);
            //}
        }
    }
}
