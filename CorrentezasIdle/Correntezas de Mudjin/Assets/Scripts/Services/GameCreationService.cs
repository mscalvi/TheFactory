using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class GameCreationService : MonoBehaviour
{
    private GameState GameState;
    private GameDatabase DataBase;

    public void Initialize(GameState gs, GameDatabase db)
    {
        GameState = gs;
        DataBase = db;

        GameState.CompanyCurrency = new Dictionary<CurrencyHelper.CurrencyType, CurrencyInstance>();
        GameState.CompanyUpgrades = new Dictionary<string, UpgradeInstance>();

        CreateDataState(db);
        BuildShips();
        BuildCurrencies();
        BuildBuildings();
        BuildCompanyUpgrades();
        BuildDestinations();

        GameState.CurrentBase = GameState.DataState.destinations.GetValueOrDefault("d101");
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
        var upgrades = new Dictionary<string, UpgradeInstance>();
        var buildings = new Dictionary<string, BuildingInstance>();

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

        foreach (var upgrade in DataBase.upgrade)
        {
            var instance = new UpgradeInstance(upgrade);
            upgrades.Add(upgrade.Id, instance);
        }

        foreach (var building in DataBase.buildings)
        {
            var instance = new BuildingInstance(building);
            buildings.Add(building.Id, instance);
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
        GameState.DataState.upgrades = upgrades;
        GameState.DataState.buildings = buildings;
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

    private void BuildBuildings()
    {
        foreach (var building in GameState.DataState.buildings)
        {
            building.Value.Upgrades = new List<UpgradeInstance>();

            foreach (var upgrade in GameState.DataState.upgrades)
            {
                if (upgrade.Value.Building != building.Value.Type)
                    continue;

                building.Value.Upgrades.Add(upgrade.Value);
            }
        }
    }

    private void BuildCurrencies()
    {
        foreach (var currency in GameState.DataState.currencies)
        {
            if (currency.Value.UnlockStatus == UnlockHelper.UnlockStatus.Unlocked)
            {
                GameState.CompanyCurrency.Add(currency.Value.Type, currency.Value);
            }
        }
    }

    private void BuildCompanyUpgrades()
    {
        foreach (var upgrade in GameState.DataState.upgrades)
        {
            if (upgrade.Value.UnlockStatus == UnlockHelper.UnlockStatus.Unlocked)
            {
                GameState.CompanyUpgrades.Add(upgrade.Value.Id, upgrade.Value);
            }
        }
    }

    private void BuildDestinations()
    {
        foreach (var destination in GameState.DataState.destinations)
        {
            foreach (var close in destination.Value.CloseDestinationsList)
            {
                var pathId = PathLocator(destination.Key, close);

                var target = GameState.DataState.destinations.GetValueOrDefault(close);
                var path = GameState.DataState.paths.GetValueOrDefault(pathId);

                if (target == null)
                {
                    continue;
                }

                if (path == null)
                {
                    continue;
                }

                destination.Value.CloseDestinations.Add(target, path);
            }
        }
    }

    private string PathLocator(string origin, string destiny)
    {
        foreach (var path in GameState.DataState.paths)
        {
            var p = path.Value;


            bool forward = p.Destination1.Id == origin && p.Destination2.Id == destiny;
            bool backward = p.Destination2.Id == origin && p.Destination1.Id == destiny;

            if (forward || backward)
                return path.Key;
        }

        return null;
    }
}
