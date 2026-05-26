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
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects
        };

        string json = JsonConvert.SerializeObject(GameState, settings);
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

        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.Objects
        };

        GameState gameState = JsonConvert.DeserializeObject<GameState>(json, settings);

        Debug.Log("Jogo Carregado e Referências Restauradas");

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