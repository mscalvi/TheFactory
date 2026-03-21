using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public GameDatabase Database;

    public RunController CurrentRun;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        bool isFirstRun = PlayerPrefs.GetInt("HasInitialized", 0) == 0;

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        bool isFirstRun = PlayerPrefs.GetInt("HasInitialized", 0) == 0;

        if (isFirstRun)
        {
            CreateDefaultRun();

            PlayerPrefs.SetInt("HasInitialized", 1);
            PlayerPrefs.Save();
        }
    }

    void CreateDefaultRun()
    {
        var run = new RunController();

        Debug.Log(Database);
        Debug.Log(Database.ships.Length);
        Debug.Log(Database.ships[0]);

        var ship = Database.ships[0];
        Debug.Log(ship);
        run.Ship = ship;

        // Weapon Rooms
        foreach (var room in ship.WeaponRooms)
        {
            var config = new RoomConfiguration();

            Debug.Log(room.RoomModel.Id);
            config.RoomId = room.RoomModel.Id;

            Debug.Log(Database.tripulation[0]);
            config.Tripulation = Database.tripulation[0];
            Debug.Log(Database.weapons[0]);
            config.Weapon = Database.weapons[0];
            Debug.Log(Database.ammos[0]);
            config.Ammo = Database.ammos[0];

            run.Rooms.Add(config);
        }

        CurrentRun = run;
    }
}