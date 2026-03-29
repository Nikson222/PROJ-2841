using UnityEditor;
using UnityEngine;

namespace _Scripts._Infrastructure.Tools
{
    public static class SaveLoadEditorTool
    {
        #if UNITY_EDITOR
        [MenuItem("Tools/Delete All Saves")]
        public static void DeleteAllSaves()
        {
            string[] saveFiles = System.IO.Directory.GetFiles(Application.persistentDataPath);

            foreach (var file in saveFiles)
            {
                System.IO.File.Delete(file);
                Debug.Log("Deleted save file: " + file);
            }

            Debug.Log("All save files have been deleted.");
        }
        #endif
    }
}