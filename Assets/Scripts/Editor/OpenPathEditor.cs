using UnityEngine;
using UnityEditor;
using System.IO;

public class OpenPathEditor : EditorWindow {
    [MenuItem("Tools/Open Persistent Data Path")]
    public static void OpenPersistentData() {
        string persistentDataPath = Application.persistentDataPath;

        if (Directory.Exists(persistentDataPath)) {
            // Open Windows Explorer with the persistent data path
            OpenInFileBrowser(persistentDataPath);
        }
        else {
            Debug.LogWarning("Persistent data path not found or inaccessible.");
        }
    }

    // Helper method to open Windows Explorer at a specific path
    private static void OpenInFileBrowser(string path) {
        path = path.Replace(@"/", @"\");   // Unity paths use '/', but Explorer needs '\'
        System.Diagnostics.Process.Start("explorer.exe", "/select," + path);
    }
}