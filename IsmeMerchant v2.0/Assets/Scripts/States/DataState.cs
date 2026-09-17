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
    public Dictionary<AlchemyHelper.IngredientType, IngredientInstance> ingredients;

    public Dictionary<string, UpgradeInstance> upgrades;

    public Dictionary<string, BuildingInstance> buildings;

    public Dictionary<string, LabInstance> labs;
    public Dictionary<string, ProductInstance> products;

    public Dictionary<string, MissionInstance> missions;
}
