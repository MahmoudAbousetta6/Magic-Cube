using System.IO;
using UnityEngine;

namespace Scripts
{
    /// <summary>
    /// Handle saving and loading game in Json file.
    /// </summary>
    public static class SaveStateHandler
    {
        private static string path = string.Empty;
        private static string persistentPath = string.Empty;

        private static void SetPath()
        {
            path = Application.dataPath + Path.AltDirectorySeparatorChar + "SaveData.json";
            persistentPath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "SaveData.json";
        }

        public static void SaveData(MagicCubeInfo magicCubeInfo)
        {
            SetPath();
            var savePath = persistentPath;

            var json = JsonUtility.ToJson(magicCubeInfo);
            Debug.Log(json);

            using var writer = new StreamWriter(savePath);
            writer.Write(json);
        }

        public static MagicCubeInfo LoadData()
        {
            SetPath();
            using var reader = new StreamReader(persistentPath);
            var json = reader.ReadToEnd();

            var data = JsonUtility.FromJson<MagicCubeInfo>(json);
            return data;
        }
    }
}