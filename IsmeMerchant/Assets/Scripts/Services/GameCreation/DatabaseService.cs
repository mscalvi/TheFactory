using UnityEngine;

public class DatabaseService : MonoBehaviour
{
    public GameDatabase Database;

    public GameDatabase Initialize()
    {
        Database = new GameDatabase();

        TripulationData.Load();
        EnemiesData.Load();

        ShipData.Load();
        WeaponsData.Load();
        AmmoData.Load();
        ProjectilesData.Load();

        ConstructionData.Load();
        BuildingsData.Load();
        UpgradesData.Load();

        CurrenciesData.Load();
        IngredientsData.Load();

        EventsData.Load();
        MissionsData.Load();

        Database.tripulations = TripulationData.All;
        Database.enemies = EnemiesData.All;

        Database.ships = ShipData.All;
        Database.weapons = WeaponsData.All;
        Database.ammos = AmmoData.All;
        Database.projectiles = ProjectilesData.All;

        Database.constructions = ConstructionData.All;
        Database.buildings = BuildingsData.All;
        Database.upgrades = UpgradesData.All;

        Database.currencies = CurrenciesData.All;
        Database.ingredients = IngredientsData.All;

        Database.events = EventsData.All;
        Database.missions = MissionsData.All;

        return Database;
    }  
}