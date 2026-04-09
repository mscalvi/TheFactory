using UnityEngine;

[CreateAssetMenu(menuName = "Game/Game Database")]
public class GameDatabase : ScriptableObject
{
    public ShipModel[] ships;

    public WeaponModel[] weapons;
    public AmmoModel[] ammos;

    public TripulationModel[] tripulation;

    public WeaponRoomModel[] weaponsRooms;
    public OtherRoomModel[] otherRooms;

    public EnemyModel[] enemies;

    public DestinationModel[] destinations;
    public PathModel[] paths;

    public CurrencyModel[] currency;
    public IngredientModel[] ingredients;

    public UpgradeModel[] upgrade;

    public BuildingModel[] buildings;
}