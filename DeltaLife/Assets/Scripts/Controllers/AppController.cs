using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppController : MonoBehaviour
{
    public static AppController Instance;

    public AppState AppState;
    //public GameDatabase Database;

    [SerializeField] AppCreationService AppCreationService;
    [SerializeField] public DataService DataService;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        //if (Database == null)
        //{
        //    Database = DatabaseService.Initialize();
        //}

        //VersionService.CheckVersion();
        //AppState = SaveService.Load();
        Debug.Log(Application.persistentDataPath);

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        if (AppState == null)
        {
            AppState = new AppState();

            AppCreationService.Initialize(AppState);
            DataService.Initialize(AppState);
        }
    }
}
