using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppCreationService : MonoBehaviour
{
    private AppState AppState;
    //private GameDatabase DataBase;

    public void Initialize(AppState gs)
    {
        AppState = gs;
        //DataBase = db;

        //CreateDataState(db);
    }
}
