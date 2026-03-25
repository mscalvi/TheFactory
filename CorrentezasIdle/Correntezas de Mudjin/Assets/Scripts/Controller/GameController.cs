using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public GameState GameState;

    public GameDatabase Database;

    public LandingService LandingService;

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
        }

        LandingService.Initialize(GameState, Database);
    }
}