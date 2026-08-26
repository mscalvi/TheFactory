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

        Debug.Log($"Versão atual: {currentVersion}");
        Debug.Log($"Última versão: {lastVersion}");

        if (string.IsNullOrEmpty(lastVersion))
        {
            SaveVersion(currentVersion);
            return;
        }

        if (IsNewerVersion(currentVersion, lastVersion))
        {
            Debug.Log(
                $"Atualização detectada: {lastVersion} -> {currentVersion}"
            );

            ResetGame();

            SaveVersion(currentVersion);
        }
    }

    private bool IsNewerVersion(string current, string previous)
    {
        return new System.Version(current) >
               new System.Version(previous);
    }

    private void SaveVersion(string version)
    {
        PlayerPrefs.SetString(VersionKey, version);
        PlayerPrefs.Save();
    }

    private void ResetGame()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.Log("Save apagado devido à atualização.");
        }
    }
}