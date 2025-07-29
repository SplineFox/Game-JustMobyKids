using System.IO;
using UnityEngine;

public static class SaveUtils
{
    public static string GetRootPath()
    {
        return Application.persistentDataPath;
#if UNITY_EDITOR
        return Path.Combine(Application.dataPath, "..", "SaveData");
#endif
    }

    public static string GetFilePath(string filename)
    {
        var rootPath = GetRootPath();
        return Path.Combine(rootPath, filename);
    }
}