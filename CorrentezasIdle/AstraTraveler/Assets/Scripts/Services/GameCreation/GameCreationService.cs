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

        CreateDataState(db);
        BuildShips();
        BuildIngredients();
        BuildBestiary();
        BuildTripulation();
        BuildAmmos();
    }

    private void CreateDataState(GameDatabase DataBase)
    {
        var ships = new Dictionary<string, ShipInstance>();
        var tripulation = new Dictionary<string, TripulationInstance>();
        var weapons = new Dictionary<string, WeaponInstance>();
        var ammos = new Dictionary<string, AmmoInstance>();
        var projectiles = new Dictionary<string, ProjectileInstance>();
        var enemies = new Dictionary<string, EnemyInstance>();     
        var currencies = new Dictionary<CurrencyHelper.CurrencyType, CurrencyInstance>();
        var ingredients = new Dictionary<IngredientHelper.IngredientType, IngredientInstance>();
        var upgrades = new Dictionary<string, UpgradeInstance>();
        var acquisitions = new Dictionary<string, AcquisitionInstance>();
        var buildings = new Dictionary<string, BuildingInstance>();
        var events = new Dictionary<string, EventInstance>();
        var missions = new Dictionary<string, MissionInstance>();

        foreach (var ship in DataBase.ships.Values)
        {
            var instance = new ShipInstance(ship);
            ships.Add(ship.Id, instance);
        }

        foreach (var trip in DataBase.tripulations.Values)
        {
            var instance = new TripulationInstance(trip);
            tripulation.Add(trip.Id, instance);
        }

        foreach (var weapon in DataBase.weapons.Values)
        {
            var instance = new WeaponInstance(weapon);
            weapons.Add(weapon.Id, instance);
        }

        foreach (var ammo in DataBase.ammos.Values)
        {
            var instance = new AmmoInstance(ammo);
            ammos.Add(ammo.Id, instance);
        }

        foreach (var projectile in DataBase.projectiles.Values)
        {
            var instance = new ProjectileInstance(projectile);
            projectiles.Add(projectile.Id, instance);
        }

        foreach (var enemy in DataBase.enemies.Values)
        {
            var instance = new EnemyInstance(enemy);
            enemies.Add(enemy.Id, instance);
        }

        foreach (var currency in DataBase.currencies.Values)
        {
            var instance = new CurrencyInstance(currency);
            currencies.Add(currency.Type, instance);
        }

        foreach (var ingrediente in DataBase.ingredients.Values)
        {
            var instance = new IngredientInstance(ingrediente);
            ingredients.Add(ingrediente.Type, instance);
        }

        foreach (var upgrade in DataBase.upgrades.Values)
        {
            var instance = new UpgradeInstance(upgrade);
            upgrades.Add(upgrade.Id, instance);
        }

        foreach (var acquisition in DataBase.acquisitions.Values)
        {
            var instance = new AcquisitionInstance(acquisition);
            acquisitions.Add(acquisition.Id, instance);
        }

        foreach (var building in DataBase.buildings.Values)
        {
            var instance = new BuildingInstance(building);
            buildings.Add(building.Id, instance);
        }

        foreach (var eventModel in DataBase.events.Values)
        {
            var instance = new EventInstance(eventModel);
            events.Add(eventModel.Id, instance);
        }

        foreach (var missiomModel in DataBase.missions.Values)
        {
            var instance = new MissionInstance(missiomModel);
            missions.Add(missiomModel.Id, instance);
        }

        GameState.DataState.ships = ships;
        GameState.DataState.tripulations = tripulation;
        GameState.DataState.weapons = weapons;
        GameState.DataState.ammos = ammos;
        GameState.DataState.projectiles = projectiles;
        GameState.DataState.enemies = enemies;
        GameState.DataState.currencies = currencies;
        GameState.DataState.ingredients = ingredients;
        GameState.DataState.upgrades = upgrades;
        GameState.DataState.acquisitions = acquisitions;
        GameState.DataState.buildings = buildings;
        GameState.DataState.events = events;
        GameState.DataState.missions = missions;
    }

    private void BuildShips()
    {                
        GameState.ExpeditionState.Ship = GameState.DataState.ships["s001"];
        GameState.ExpeditionState.Ship.Weapons.Add(GameState.DataState.weapons["w001"]);
        GameState.ExpeditionState.Ship.Weapons[0].Ammo = GameState.DataState.ammos["a001"];
    }

    private void BuildIngredients()
    {
        GameState.ExpeditionState.StartIngredientRarityWeights = new Dictionary<GameHelper.ItemRarity, float>()
        {
            { GameHelper.ItemRarity.Common, 100 },
            { GameHelper.ItemRarity.Uncommon, 0 },
            { GameHelper.ItemRarity.Rare, 0 },
            { GameHelper.ItemRarity.Legendary, 0 }
        };

        GameState.ExpeditionState.BaseIngredientRarityWeights = GameState.ExpeditionState.StartIngredientRarityWeights;
        GameState.ExpeditionState.ActualIngredientRarityWeights = GameState.ExpeditionState.StartIngredientRarityWeights;
    }

    private void BuildBestiary()
    {
        foreach (var enemy in GameState.DataState.enemies)
        {
            GameState.BestiaryState.Bestiary.Add(enemy.Value.Id, new BestiaryEntry());
        }
    }

    private void BuildTripulation()
    {
        GameState.ExpeditionState.ActiveTripulation.Add(GameState.DataState.tripulations["t001"]);
    }

    private void BuildAmmos()
    {
        foreach (var ammo in GameState.DataState.ammos.Values)
        {
            ammo.Projectile = GameState.DataState.projectiles["r001"];
        }
    }
}
