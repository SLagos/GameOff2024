using Unity.Netcode;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public class PlayModeSceneLoader
{
    private const string InitSceneName = "Init";
    private static string previousScenePath;

    static PlayModeSceneLoader()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            // Store the current scene path
            previousScenePath = SceneManager.GetActiveScene().path;

            // Save the current scene
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

            // Load the Init scene
            EditorSceneManager.OpenScene($"Assets/Scenes/{InitSceneName}.unity");
        }
        else if (state == PlayModeStateChange.EnteredEditMode && !string.IsNullOrEmpty(previousScenePath))
        {
            // Load the previous scene after exiting Play mode
            EditorSceneManager.OpenScene(previousScenePath);
        }
        else if (state == PlayModeStateChange.ExitingPlayMode)
        {
            NetworkManager.Singleton.Shutdown();
        }
    }
}
