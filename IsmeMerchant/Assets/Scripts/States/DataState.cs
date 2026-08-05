using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataState
{
    public Dictionary<string, ShipInstance> ships;

    public Dictionary<string, WeaponInstance> weapons;
    public Dictionary<string, AmmoInstance> ammos;

    public Dictionary<string, EnemyInstance> enemies;
    
    public Dictionary<CurrencyHelper.CurrencyType, CurrencyInstance> currencies;
    public Dictionary<IngredientHelper.IngredientType, IngredientInstance> ingredients;

    public Dictionary<string, UpgradeInstance> upgrades;

    public Dictionary<string, BuildingInstance> buildings;

    public Dictionary<string, MissionInstance> missions;
}
