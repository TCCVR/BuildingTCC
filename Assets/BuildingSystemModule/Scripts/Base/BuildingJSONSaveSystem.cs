using System.IO;
using UnityEngine;

namespace BuildingSystem {
    public static class BuildingJSONSaveSystem {

        private static readonly string SAVE_FOLDER = Path.Combine(Application.dataPath, "Saves");
        private const string SAVE_EXTENSION = "json";

        public static void Init() {
            if (!Directory.Exists(SAVE_FOLDER)) {
                Directory.CreateDirectory(SAVE_FOLDER);
            }
        }

        public static void Save(string saveString) {
            // Make sure the Save Number is unique so it doesnt overwrite a previous save file
            int saveNumber = 1;
            while (File.Exists(Path.Combine(SAVE_FOLDER, "save_" + saveNumber + "." + SAVE_EXTENSION))) {
                saveNumber++;
            }
            // saveNumber is unique
            File.WriteAllText(Path.Combine(SAVE_FOLDER, "save_" + saveNumber + "." + SAVE_EXTENSION), saveString);
        }

        public static string Load() {
            DirectoryInfo directoryInfo = new DirectoryInfo(SAVE_FOLDER);
            // Get all save files
            FileInfo[] saveFiles = directoryInfo.GetFiles("*." + SAVE_EXTENSION);
            // Cycle through all save files and identify the most recent one
            FileInfo mostRecentFile = null;
            foreach (FileInfo fileInfo in saveFiles) {
                if (mostRecentFile == null) {
                    mostRecentFile = fileInfo;
                }
                else {
                    if (fileInfo.LastWriteTime > mostRecentFile.LastWriteTime) {
                        mostRecentFile = fileInfo;
                    }
                }
            }

            // If theres a save file, load it, if not return null
            if (mostRecentFile != null) {
                string saveString = File.ReadAllText(mostRecentFile.FullName);
                return saveString;
            }
            else {
                return null;
            }
        }

    }
}
