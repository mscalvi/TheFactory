using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class SaveService : MonoBehaviour
{
    private GameState GameState;

    private string SavePath =>
        Path.Combine(Application.persistentDataPath, "save.json");

    public void Initialize(GameState gameState)
    {
        GameState = gameState;
    }

    public void Save()
    {
        string json =
            JsonConvert.SerializeObject(GameState, Formatting.Indented);

        File.WriteAllText(SavePath, json);

        Debug.Log("Jogo Salvo");
    }

    public GameState Load()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("Nenhum Jogo Salvo Encontrado");
            return null;
        }

        string json = File.ReadAllText(SavePath);

        GameState gameState = JsonConvert.DeserializeObject<GameState>(json);

        Debug.Log("Jogo Carregado");

        gameState.ExpeditionState.ActiveEnemies.Clear();

        return gameState;
    }

    private void OnApplicationQuit()
    {
        if (GameState.ExpeditionState.ExpeditionStatus != GameHelper.ExpeditionStatus.Running)
        {
            Save();
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            if (GameState.ExpeditionState.ExpeditionStatus != GameHelper.ExpeditionStatus.Running)
            {
                Save();
            }
        }
    }
}