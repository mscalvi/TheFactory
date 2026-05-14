using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseService : MonoBehaviour
{
    public GameDatabase Initialize()
    {
        var db = new GameDatabase();

        db.tripulations = TripulationData.All;

        return db;
    }
}
