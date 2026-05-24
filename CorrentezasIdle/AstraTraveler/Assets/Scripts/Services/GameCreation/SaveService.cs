using System.IO;
using UnityEngine;

public class SaveService : MonoBehaviour
{
    private GameState GameState;

    public void Initialize(GameState gameState)
    {
        GameState = gameState;
    }

    private string SavePath =>
        Path.Combine(Application.persistentDataPath, "save.json");

    public void Save()
    {
        string json = JsonUtility.ToJson(GameState, true);

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

        GameState gameState =
            JsonUtility.FromJson<GameState>(json);

        Debug.Log("Jogo Carregado");

        return gameState;
    }

    private void OnApplicationQuit()
    {
        Save();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Save();
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            Save();
        }
    }
}