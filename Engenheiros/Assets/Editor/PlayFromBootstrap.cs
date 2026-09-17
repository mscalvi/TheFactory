using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class PlayFromBootstrap
{
    private const string bootstrapScene = "Assets/Scenes/BootstrapScene.unity";

    static PlayFromBootstrap()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            var activeScene = SceneManager.GetActiveScene();

            if (activeScene.path != bootstrapScene)
            {
                // Salva mudanças antes de trocar
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(bootstrapScene);
                }
                else
                {
                    // Cancela o Play se o usuário não quiser salvar
                    EditorApplication.isPlaying = false;
                }
            }
        }
    }
}