using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SQLite;
using UnityEngine;

public class DataService : MonoBehaviour
{
    private AppState AppState;
    private SQLiteConnection Database;


    public void Initialize(AppState app)
    {
        AppState = app;

        string databasePath =
            System.IO.Path.Combine(
                Application.persistentDataPath,
                "MTGLife.db"
            );

        Database = new SQLiteConnection(databasePath);

        Debug.Log($"Database: {databasePath}");

        Database.CreateTable<PlayerModel>();
        Database.CreateTable<DeckModel>();
    }

    // Players
    public void CreatePlayer(PlayerModel player)
    {
        Database.Insert(player);
    }

    public List<PlayerModel> GetPlayers()
    {
        return Database.Table<PlayerModel>().ToList();
    }

    // Decks
    public void CreateDeck(DeckModel deck)
    {
        Database.Insert(deck);
    }

    public List<DeckModel> GetDecks()
    {
        return Database.Table<DeckModel>().ToList();
    }
}
