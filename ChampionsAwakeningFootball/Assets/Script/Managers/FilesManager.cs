using System.IO;
using UnityEngine;
using Mono.Data.Sqlite;

public class FilesManager : MonoBehaviour
{
    private string dbName;
    private string cardsFile;

    void Start()
    {
        // Sp�cifie le chemin de du dossier StreamingAssets et PersistantDataPath
        string streamingAssetsPath = Application.streamingAssetsPath;
        string persistentDataPath = Application.persistentDataPath;


        foreach (string sourceFilePath in Directory.GetFiles(streamingAssetsPath))
        {
            string fileName = Path.GetFileName(sourceFilePath);
            string destFilePath = Path.Combine(persistentDataPath, fileName);

            if (!(fileName.Substring(fileName.Length - 4, 4) == "meta"))
                CopyFile(sourceFilePath, destFilePath);
        }
        foreach (string sourceFolderPath in Directory.GetDirectories(streamingAssetsPath))
        {
            string folderName = Path.GetFileName(sourceFolderPath);
            string destFolderPath = Path.Combine(persistentDataPath, folderName);

            CreateFolder(destFolderPath);
            foreach (string sourceFilePath in Directory.GetFiles(sourceFolderPath))
            {
                string fileName = Path.GetFileName(sourceFilePath);
                string destFilePath = Path.Combine(destFolderPath, fileName);

                if(!(fileName.Substring(fileName.Length-4,4) == "meta"))
                    CopyFile(sourceFilePath, destFilePath);
            }
        }
    }

    // M�thode pour copier le fichier depuis StreamingAssets vers persistentDataPath
    private void CopyFile(string sourcePath, string destinationPath)
    {
        // V�rifie si le fichier existe d�j� dans le r�pertoire de destination
        if (File.Exists(destinationPath))
        {
            Debug.Log("Le fichier existe d�j� dans persistentDataPath.");
            return;
        }

        // V�rifie si le fichier existe dans StreamingAssets
        if (File.Exists(sourcePath))
        {
            // Copie le fichier depuis StreamingAssets vers persistentDataPath
            File.Copy(sourcePath, destinationPath);
            Debug.Log("Fichier copi�e dans persistentDataPath.");
        }
        else
        {
            Debug.LogError("Le fichier source n'existe pas dans StreamingAssets.");
        }
    }

    private void CreateFolder(string destinationPath)
    {
        if (Directory.Exists(destinationPath))
        {
            Debug.Log("Le dossier existe d�j� dans persistentDataPath.");
            return;
        }

        Directory.CreateDirectory(destinationPath);
        Debug.Log("Dossier cr�� dans persistentDataPath.");
    }
}

