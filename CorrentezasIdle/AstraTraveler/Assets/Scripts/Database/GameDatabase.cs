using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDatabase
{
    public Dictionary<string, TripulationModel> tripulations;

    public Dictionary<string, ShipModel> ships;
    public Dictionary<string, WeaponModel> weapons;
    public Dictionary<string, AmmoModel> ammos;
    public Dictionary<string, ProjectileModel> projectiles;

    public Dictionary<string, EnemyModel> enemies;

    public Dictionary<string, CurrencyModel> currencies;
    public Dictionary<string, IngredientModel> ingredients;

    public Dictionary<string, UpgradeModel> upgrades;
    public Dictionary<string, ConstructionModel> constructions;

    public Dictionary<string, BuildingModel> buildings;

    public Dictionary<string, EventModel> events;
    public Dictionary<string, MissionModel> missions;
}
