using UnityEngine;

public static class ProgressFileSaver {
    public static void SaveProgress(ProgressState data) {
        Save<ProgressState>(data, ProgressFilePath);
    }
    public static ProgressState LoadProgress() {
        return System.IO.File.Exists(ProgressFilePath) ? Load<ProgressState>(ProgressFilePath) : new ProgressState();
    }
    public static PlayerSettings LoadSettings() {
        return System.IO.File.Exists(SettingsFilePath) ? Load<PlayerSettings>(SettingsFilePath) : new PlayerSettings();
    }
    public static void SaveSettings(PlayerSettings data) {
        Save<PlayerSettings>(data, SettingsFilePath);
    }
    static void Save<T>(T dataToSave, string path) {
        using (System.IO.StreamWriter writer = new System.IO.StreamWriter(path)) {
            string dataToWrite = JsonUtility.ToJson(dataToSave);
            writer.Write(dataToWrite);
        }
    }
    static T Load<T>(string path) {
        T dataObject;
        using (System.IO.StreamReader reader = new System.IO.StreamReader(path)) {
            string dataToLoad = reader.ReadToEnd();
            dataObject = JsonUtility.FromJson<T>(dataToLoad);
        }
        return dataObject;
    }
    static string ProgressFilePath = System.IO.Path.Combine(Application.persistentDataPath, "amogusProgress.json");
    static string SettingsFilePath = System.IO.Path.Combine(Application.persistentDataPath, "amogusSettings.json");
}