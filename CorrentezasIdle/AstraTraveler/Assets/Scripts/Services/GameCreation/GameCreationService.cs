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
        BuildBuildings();
        BuildBestiary();
    }

    private void CreateDataState(GameDatabase DataBase)
    {
        GameState.DataState = new DataState();

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

        foreach (var projectile in DataBase.projectiles)
        {
            var instance = new ProjectileInstance(projectile);
            projectiles.Add(projectile.Id, instance);
        }

        foreach (var enemy in DataBase.enemies)
        {
            var instance = new EnemyInstance(enemy);
            enemies.Add(enemy.Id, instance);
        }

        foreach (var currency in DataBase.currency)
        {
            var instance = new CurrencyInstance(currency);
            currencies.Add(currency.Type, instance);
        }

        foreach (var ingrediente in DataBase.ingredients)
        {
            var instance = new IngredientInstance(ingrediente);
            ingredients.Add(ingrediente.Type, instance);
        }

        foreach (var upgrade in DataBase.upgrades)
        {
            var instance = new UpgradeInstance(upgrade);
            upgrades.Add(upgrade.Id, instance);
        }

        foreach (var acquisition in DataBase.acquisition)
        {
            var instance = new AcquisitionInstance(acquisition);
            acquisitions.Add(acquisition.Id, instance);
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
        GameState.ExpeditionState.Ship.Weapons = new List<WeaponInstance>();
        GameState.ExpeditionState.Ship.Weapons.Add(GameState.DataState.weapons["w001"]);
        GameState.ExpeditionState.Ship.Weapons[0].Ammo = GameState.DataState.ammos["a001"];
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

    private void BuildIngredients()
    {
        GameState.ExpeditionState.IngredientRarityBaseWeights = new Dictionary<IngredientHelper.IngredientRarity, float>()
        {
            { IngredientHelper.IngredientRarity.Common, 100 },
            { IngredientHelper.IngredientRarity.Uncommon, 0 },
            { IngredientHelper.IngredientRarity.Rare, 0 },
            { IngredientHelper.IngredientRarity.Legendary, 0 }
        };
    }

    private void BuildBestiary()
    {
        foreach (var enemy in GameState.DataState.enemies)
        {
            GameState.BestiaryState.Bestiary.Add(enemy.Value.Id, new BestiaryEntry());
        }
    }
}
