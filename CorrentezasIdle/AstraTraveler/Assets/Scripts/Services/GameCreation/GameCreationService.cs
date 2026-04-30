using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
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

        GameState.CompanyState.CompanyCurrency = new Dictionary<CurrencyHelper.CurrencyType, CurrencyInstance>();
        GameState.CompanyState.CompanyIngredients = new Dictionary<IngredientHelper.IngredientType, IngredientInstance>();
        GameState.CompanyState.CompanyUpgrades = new Dictionary<string, UpgradeInstance>();

        CreateDataState(db);
        BuildShips();
        BuildCurrencies();
        BuildIngredients();
        BuildBuildings();
        BuildCompanyUpgrades();
        BuildDestinations();
        BuildBestiary();
    }

    private void CreateDataState(GameDatabase DataBase)
    {
        GameState.DataState = new DataState();

        var ships = new Dictionary<string, ShipInstance>();
        var tripulation = new Dictionary<string, TripulationInstance>();
        var weapons = new Dictionary<string, WeaponInstance>();
        var ammos = new Dictionary<string, AmmoInstance>();
        var enemies = new Dictionary<string, EnemyInstance>();        
        var destinations = new Dictionary<string, DestinationInstance>();
        var paths = new Dictionary<string, PathInstance>();
        var currencies = new Dictionary<string, CurrencyInstance>();
        var ingredients = new Dictionary<string, IngredientInstance>();
        var upgrades = new Dictionary<string, UpgradeInstance>();
        var buildings = new Dictionary<string, BuildingInstance>();
        var events = new Dictionary<string, EventInstance>();
        var missions = new Dictionary<string, MissionInstance>();

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

        foreach (var ingrediente in DataBase.ingredients)
        {
            var instance = new IngredientInstance(ingrediente);
            ingredients.Add(ingrediente.Id, instance);
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

        foreach (var eventModel in DataBase.events)
        {
            var instance = new EventInstance(eventModel);
            events.Add(eventModel.Id, instance);
        }

        foreach (var missiomModel in DataBase.missions)
        {
            var instance = new MissionInstance(missiomModel);
            missions.Add(missiomModel.Id, instance);
        }

        GameState.DataState.ships = ships;
        GameState.DataState.tripulations = tripulation;
        GameState.DataState.weapons = weapons;
        GameState.DataState.ammos = ammos;
        GameState.DataState.enemies = enemies;
        GameState.DataState.destinations = destinations;
        GameState.DataState.paths = paths;
        GameState.DataState.currencies = currencies;
        GameState.DataState.ingredients = ingredients;
        GameState.DataState.upgrades = upgrades;
        GameState.DataState.buildings = buildings;
        GameState.DataState.events = events;
        GameState.DataState.missions = missions;
    }

    private void BuildShips()
    {                
        GameState.ExpeditionState.Ship = GameState.DataState.ships["s001"];
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
            GameState.CompanyState.CompanyCurrency.Add(currency.Value.Type, currency.Value);
        }
    }

    private void BuildIngredients()
    {
        foreach (var ingredient in GameState.DataState.ingredients)
        {
            GameState.CompanyState.CompanyIngredients.Add(ingredient.Value.Type, ingredient.Value);
        }

        GameState.ExpeditionState.IngredientRarityBaseWeights = new Dictionary<IngredientHelper.IngredientRarity, float>()
        {
            { IngredientHelper.IngredientRarity.Common, 100 },
            { IngredientHelper.IngredientRarity.Uncommon, 0 },
            { IngredientHelper.IngredientRarity.Rare, 0 },
            { IngredientHelper.IngredientRarity.Legendary, 0 }
        };
    }

    private void BuildCompanyUpgrades()
    {
        foreach (var upgrade in GameState.DataState.upgrades)
        {
            GameState.CompanyState.CompanyUpgrades.Add(upgrade.Value.Id, upgrade.Value);
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

    private void BuildBestiary()
    {
        foreach (var enemy in GameState.DataState.enemies)
        {
            GameState.BestiaryState.Bestiary.Add(enemy.Value.Id, new BestiaryEntry());
        }
    }
}
