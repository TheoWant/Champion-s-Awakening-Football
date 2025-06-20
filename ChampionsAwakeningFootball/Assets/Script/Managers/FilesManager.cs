using System.IO;
using UnityEngine;
using System.Threading.Tasks;

public class FilesManager : MonoBehaviour
{
    async void Start()
    {
        await LoadFiles();
    }

    async Task LoadFiles()
    {
        string streamingAssetsPath = Application.streamingAssetsPath;
        string persistentDataPath = Application.persistentDataPath;

        // Copier les fichiers à la racine
        string[] files = await Task.Run(() => Directory.GetFiles(streamingAssetsPath));
        foreach (var sourceFilePath in files)
        {
            string fileName = Path.GetFileName(sourceFilePath);
            if (fileName.EndsWith(".meta")) continue;

            string destFilePath = Path.Combine(persistentDataPath, fileName);
            await CopyFile(sourceFilePath, destFilePath);
        }

        // Copier les dossiers et leur contenu
        string[] directories = await Task.Run(() => Directory.GetDirectories(streamingAssetsPath));
        foreach (var sourceFolderPath in directories)
        {
            string folderName = Path.GetFileName(sourceFolderPath);
            string destFolderPath = Path.Combine(persistentDataPath, folderName);

            await CreateFolder(destFolderPath);

            string[] nestedFiles = await Task.Run(() => Directory.GetFiles(sourceFolderPath));
            foreach (var sourceFilePath in nestedFiles)
            {
                string fileName = Path.GetFileName(sourceFilePath);
                if (fileName.EndsWith(".meta")) continue;

                string destFilePath = Path.Combine(destFolderPath, fileName);
                await CopyFile(sourceFilePath, destFilePath);
            }
        }
    }

    private async Task CopyFile(string sourcePath, string destinationPath)
    {
        await Task.Run(() =>
        {
            if (File.Exists(destinationPath))
            {
                Debug.Log($"Fichier déjà existant : {destinationPath}");
                return;
            }

            if (File.Exists(sourcePath))
            {
                File.Copy(sourcePath, destinationPath);
                Debug.Log($"Copie : {sourcePath} ? {destinationPath}");
            }
            else
            {
                Debug.LogError($"Fichier source introuvable : {sourcePath}");
            }
        });
    }

    private async Task CreateFolder(string path)
    {
        await Task.Run(() =>
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Debug.Log($"Dossier créé : {path}");
            }
            else
            {
                Debug.Log($"Dossier existant : {path}");
            }
        });
    }
}
