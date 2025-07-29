using System.IO;
using UnityEditor;

namespace Editor
{
    public static class SaveTools
    {
        [MenuItem("Saves/ClearSave", priority = 101)]
        public static void ClearSave()
        {
            var saveDataDirectory = new DirectoryInfo(SaveUtils.GetRootPath());
            if (saveDataDirectory.Exists)
            {
                foreach (var file in saveDataDirectory.EnumerateFiles())
                {
                    file.Delete();
                }

                foreach (var directory in saveDataDirectory.EnumerateDirectories())
                {
                    directory.Delete(true);
                }
            }
        }
    }
}