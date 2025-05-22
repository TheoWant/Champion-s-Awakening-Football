using System.IO;
using UnityEngine;
using Mono.Data.Sqlite;

public class FilesManager : MonoBehaviour
{
    private string dbName;
    private string cardsFile;

    void Start()
    {
        // Spécifie le chemin de du dossier StreamingAssets et PersistantDataPath
        string streamingAssetsPath = Path.Combine(Application.streamingAssetsPath);
        string persistantDataPath = Path.Combine(Application.persistentDataPath);


        foreach (string streaminAssetsFile  in Directory.GetFiles(streamingAssetsPath))
        {
            string file = Path.GetFileName(streaminAssetsFile);
            if (!File.Exists(persistantDataPath+"/"+file))
            {
                CopyDatabase(streaminAssetsFile, persistantDataPath + "/" + file);
            }
        }
    }

    // Méthode pour copier le fichier depuis StreamingAssets vers persistentDataPath
    private void CopyDatabase(string sourcePath, string destinationPath)
    {
        // Vérifie si le fichier existe déjà dans le répertoire de destination
        if (File.Exists(destinationPath))
        {
            Debug.Log("Le fichier existe déjà dans persistentDataPath.");
            return;
        }

        // Vérifie si le fichier existe dans StreamingAssets
        if (File.Exists(sourcePath))
        {
            // Copie le fichier depuis StreamingAssets vers persistentDataPath
            File.Copy(sourcePath, destinationPath);
            Debug.Log("Fichier copiée dans persistentDataPath.");
        }
        else
        {
            Debug.LogError("Le fichier source n'existe pas dans StreamingAssets.");
        }
    }
}

