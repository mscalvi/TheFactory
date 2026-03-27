using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public GameState GameState;
    public DataState DataState;
    public GameDatabase Database;

    [SerializeField] GameCreationService GameCreationService;

    bool FirstInitialization = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        if (Database == null)
        {
            Debug.LogError("GameDatabase NÃO atribuída no GameController!");
        }

        if (GameState == null)
        {
            GameState = new GameState();
            DataState = new DataState();
        } else
        {
            FirstInitialization = false;
        }

        if (FirstInitialization)
        {
            GameCreationService.Initialize(GameState, Database);
        } else
        {
            // service de Load
        }
    }
}