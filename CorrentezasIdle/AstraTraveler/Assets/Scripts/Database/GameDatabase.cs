using UnityEngine;

[CreateAssetMenu(menuName = "Game/GameDatabase")]
public class GameDatabase : ScriptableObject
{
    public ShipModel[] ships;

    public WeaponModel[] weapons;
    public AmmoModel[] ammos;
    public ProjectileModel[] projectiles;

    public TripulationModel[] tripulation;

    public EnemyModel[] enemies;

    public CurrencyModel[] currency;
    public IngredientModel[] ingredients;

    public UpgradeModel[] upgrades;

    public BuildingModel[] buildings;

    public EventModel[] events;
    public MissionModel[] missions;
}