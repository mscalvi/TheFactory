using System;
using System.IO;
using UnityEngine;

public class VersionService : MonoBehaviour
{
    private const string VersionKey = "InstalledVersion";

    private string SavePath =>
        Path.Combine(
            Application.persistentDataPath,
            "save.json"
        );

    public void CheckVersion()
    {
        string currentVersion = Application.version;
        string lastVersion = PlayerPrefs.GetString(VersionKey, "");

        bool saveExists = File.Exists(SavePath);

        Debug.Log($"Versão atual: {currentVersion}");
        Debug.Log($"Última versão: {lastVersion}");
        Debug.Log($"Save existe: {saveExists}");

        // =====================================================
        // PRIMEIRA EXECUÇÃO REAL
        // =====================================================

        if (string.IsNullOrEmpty(lastVersion))
        {
            // Não existe versão registrada, mas existe save.
            // Estado inconsistente: não sabemos de qual versão
            // esse save veio.
            if (saveExists)
            {
                Debug.LogWarning(
                    "Save encontrado sem versão registrada. " +
                    "Apagando save por segurança."
                );

                ResetGame();
            }

            SaveVersion(currentVersion);
            return;
        }

        // =====================================================
        // MESMA VERSÃO
        // =====================================================

        if (lastVersion == currentVersion)
        {
            Debug.Log("Versão igual. Nenhuma atualização necessária.");
            return;
        }

        // =====================================================
        // ATUALIZAÇÃO
        // =====================================================

        if (IsNewerVersion(currentVersion, lastVersion))
        {
            Debug.Log(
                $"Atualização detectada: " +
                $"{lastVersion} -> {currentVersion}"
            );

            ResetGame();

            SaveVersion(currentVersion);

            return;
        }

        // =====================================================
        // VERSÃO MAIS ANTIGA
        // =====================================================

        Debug.LogWarning(
            $"Versão atual ({currentVersion}) é mais antiga " +
            $"que a versão registrada ({lastVersion})."
        );
    }

    private bool IsNewerVersion(string current, string previous)
    {
        try
        {
            return new Version(current) > new Version(previous);
        }
        catch (Exception e)
        {
            Debug.LogWarning(
                $"Erro ao comparar versões. " +
                $"Resetando save por segurança.\n{e}"
            );

            ResetGame();

            return false;
        }
    }

    private void SaveVersion(string version)
    {
        PlayerPrefs.SetString(VersionKey, version);
        PlayerPrefs.Save();

        Debug.Log($"Versão salva: {version}");
    }

    public void ResetGame()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);

            Debug.Log("Save apagado.");
        }
        else
        {
            Debug.Log("Nenhum save para apagar.");
        }
    }
}