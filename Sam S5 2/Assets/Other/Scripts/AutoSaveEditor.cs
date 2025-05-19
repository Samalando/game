using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[InitializeOnLoad] // Automatically run when the editor starts
public class AutoSaveEditor
{
    private static float saveInterval = 60f; // Interval in seconds for autosave
    private static float timeSinceLastSave = 0f;

    // Static constructor called when the editor is loaded
    static AutoSaveEditor()
    {
        // Editor Update function to continuously check for the autosave
        EditorApplication.update += Update;
    }

    private static void Update()
    {
        // Check if we're in Play mode, don't autosave in Play mode
        if (EditorApplication.isPlaying)
            return;

        // Track the time since the last autosave
        timeSinceLastSave += Time.deltaTime;

        // Perform autosave at regular intervals
        if (timeSinceLastSave >= saveInterval)
        {
            // Check if the scene is modified before autosaving
            if (IsSceneModified())
            {
                SaveScene();
            }
            else
            {
                //Debug.Log("Autosave skipped: Scene is not modified.");
            }

            timeSinceLastSave = 0f; // Reset the timer after checking
        }
    }

    // Method to check if the scene is modified (dirty) or saved
    private static bool IsSceneModified()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        // Check if the scene has been modified (dirty)
        bool isDirty = EditorSceneManager.GetSceneByName(currentScene).isDirty;

        // Debug message to indicate scene status
        if (isDirty)
        {
            //Debug.Log("Scene is modified (dirty).");
        }
        else
        {
            //Debug.Log("Scene is clean (not modified).");
        }

        return isDirty;
    }

    // Method to save the currently active scene
    private static void SaveScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        // Check if the scene is dirty (i.e., changed since last save)
        if (EditorSceneManager.GetSceneByName(currentScene).isDirty)
        {
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
            //Debug.Log("Auto-saved scene: " + currentScene);
        }
        else
        {
            //Debug.Log("No changes to save. Scene is up to date.");
        }
    }
}
