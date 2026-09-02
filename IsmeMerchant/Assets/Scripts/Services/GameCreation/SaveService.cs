using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class SaveService : MonoBehaviour
{
    private GameState GameState;

    private string SavePath =>
        Path.Combine(
            Application.persistentDataPath,
            "save.json"
        );

    public void Initialize(GameState gameState)
    {
        GameState = gameState;
    }

    public void Save()
    {
        if (GameState == null)
        {
            Debug.LogWarning("Tentativa de salvar GameState nulo.");
            return;
        }

        try
        {
            JsonSerializerSettings settings =
                new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    PreserveReferencesHandling =
                        PreserveReferencesHandling.Objects
                };

            string json =
                JsonConvert.SerializeObject(GameState, settings);

            File.WriteAllText(SavePath, json);

            Debug.Log(
                "Jogo Salvo -> " +
                GameState.ExpeditionState.ExpeditionStatus
            );
        }
        catch (Exception e)
        {
            Debug.LogError(
                $"Erro ao salvar jogo:\n{e}"
            );
        }
    }

    public GameState Load()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("Nenhum Jogo Salvo Encontrado");

            return null;
        }

        try
        {
            string json = File.ReadAllText(SavePath);

            if (string.IsNullOrWhiteSpace(json))
            {
                Debug.LogWarning(
                    "Save encontrado, mas está vazio."
                );

                DeleteSave();

                return null;
            }

            JsonSerializerSettings settings =
                new JsonSerializerSettings
                {
                    PreserveReferencesHandling =
                        PreserveReferencesHandling.Objects
                };

            GameState gameState =
                JsonConvert.DeserializeObject<GameState>(
                    json,
                    settings
                );

            if (gameState == null)
            {
                Debug.LogWarning(
                    "Save não pôde ser convertido para GameState."
                );

                DeleteSave();

                return null;
            }

            Debug.Log(
                "Jogo Carregado -> " +
                gameState.ExpeditionState.ExpeditionStatus
            );

            // Segurança contra referências antigas.
            if (gameState.ExpeditionState != null &&
                gameState.ExpeditionState.ActiveEnemies != null)
            {
                gameState.ExpeditionState.ActiveEnemies.Clear();
            }

            return gameState;
        }
        catch (Exception e)
        {
            Debug.LogError(
                $"ERRO AO CARREGAR SAVE:\n{e}"
            );

            Debug.LogWarning(
                "Save incompatível ou corrompido. " +
                "Apagando e criando um novo jogo."
            );

            DeleteSave();

            return null;
        }
    }

    public void DeleteSave()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);

            Debug.Log("Save deletado.");
        }
    }

    private void OnApplicationQuit()
    {
        if (GameState == null ||
            GameState.ExpeditionState == null)
            return;

        if (GameState.ExpeditionState.ExpeditionStatus !=
            GameHelper.ExpeditionStatus.Running)
        {
            Save();
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (!pause)
            return;

        if (GameState == null ||
            GameState.ExpeditionState == null)
            return;

        if (GameState.ExpeditionState.ExpeditionStatus !=
            GameHelper.ExpeditionStatus.Running)
        {
            Save();
        }
    }
}