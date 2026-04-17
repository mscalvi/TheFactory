using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataState
{
    public Dictionary<string, ShipInstance> ships;

    public Dictionary<string, WeaponInstance> weapons;
    public Dictionary<string, AmmoInstance> ammos;
    
    public Dictionary<string, TripulationInstance> tripulations;

    public Dictionary<string, WeaponRoomInstance> weaponsRooms;
    // public OtherRoomInstance[] otherRooms;

    public Dictionary<string, EnemyInstance> enemies;

    public Dictionary<string, DestinationInstance> destinations;
    public Dictionary<string, PathInstance> paths;

    public Dictionary<string, CurrencyInstance> currencies;
    public Dictionary<string, IngredientInstance> ingredients;
    public Dictionary<string, UpgradeInstance> upgrades;

    public Dictionary<string, BuildingInstance> buildings;

    public Dictionary<string, EventInstance> events;

    public Dictionary<string, MissionInstance> missions;
}
