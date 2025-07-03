using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Mono.Data.Sqlite;
using TMPro;
using UnityEngine.UI;
using System.Security.Cryptography;
using System;
using UnityEditor.MemoryProfiler;
using UnityEngine.SceneManagement;

public class CareerManager : MonoBehaviour
{
    private string dbName;
    [SerializeField] List<CareerMenuDisplayUI> careerMenuDisplayUIs;
    [SerializeField] TextMeshProUGUI firstNameInputField;
    [SerializeField] TextMeshProUGUI lastNameInputField;
    [SerializeField] TextMeshProUGUI countryInputField;

    [SerializeField] GameObject CreateCareerPopUp;
    [SerializeField] GameObject DeleteCareerPopUp;

    [SerializeField] TMP_Dropdown ClubDisplayDropdown;

    [SerializeField] LoadScene loadScene;

    private int careerClickedId;


    // Start is called before the first frame update
    void Awake()
    {
        dbName = Path.Combine(Application.persistentDataPath, "Teams.db");

        UpdateCareerUI();
    }

    public void ResetVariablesValues()
    {
        firstNameInputField.text = "";
        countryInputField.text = "";
    }

    void UpdateCareerUI()
    {
        using (var connection = new SqliteConnection("URI=file:" + dbName))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                for (int i = 0; i < 3; i++)
                {
                    command.CommandText = "SELECT * FROM player WHERE id = @id;";
                    command.Parameters.AddWithValue("@id", i);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                careerMenuDisplayUIs[i]._playerName.text = reader["firstName"].ToString() + " " + reader["lastName"].ToString() + " - " + reader["club"].ToString();
                                careerMenuDisplayUIs[i]._loadCareerBtn.GetComponent<Button>().onClick.RemoveAllListeners();
                                int index = i;
                                careerMenuDisplayUIs[i]._loadCareerBtn.GetComponent<Button>().onClick.AddListener(() => SetActiveOnClick(index));
                                careerMenuDisplayUIs[i]._loadCareerBtn.GetComponent<Button>().onClick.AddListener(LoadCareer);
                                careerMenuDisplayUIs[i]._deleteCareerBtn.GetComponent<Button>().onClick.AddListener(() => SetActiveOnClick(index, DeleteCareerPopUp));
                            }
                            reader.Close();
                        }
                        else
                        {
                            careerMenuDisplayUIs[i]._playerName.text = "Emplacement de sauvegarde vide";
                            careerMenuDisplayUIs[i]._loadBtnText.text = "Commencer une carrière";
                            careerMenuDisplayUIs[i]._deleteCareerBtn.SetActive(false);
                            careerMenuDisplayUIs[i]._loadCareerBtn.GetComponent<Button>().onClick.RemoveAllListeners();
                            int index = i;
                            careerMenuDisplayUIs[index]._loadCareerBtn.GetComponent<Button>().onClick.AddListener(() => SetActiveOnClick(index, CreateCareerPopUp));

                        }
                    }
                }
            }
            connection.Close();
        }
    }

    void SetActiveOnClick(int cId, GameObject? go = null)
    {

        careerClickedId = cId;
        if(go != null )
        {
            go.SetActive(true);
        }
    }

    public void CreateCareer()
    {
        if (firstNameInputField == null || lastNameInputField == null || string.IsNullOrWhiteSpace(firstNameInputField.text) || string.IsNullOrWhiteSpace(lastNameInputField.text)) { Debug.Log("RETURN"); return; }

        using (var connection = new SqliteConnection("URI=file:" + dbName))
        {
            connection.Open();

            int countryId = 0, leagueId = 0;

            using (var command = connection.CreateCommand())
            {
                int careerTableInt = careerClickedId + 1;
                command.CommandText = $"SELECT league_id FROM teams_career_{careerTableInt} WHERE name = @ClubName;";
                command.Parameters.AddWithValue("@ClubName", ClubDisplayDropdown.options[ClubDisplayDropdown.value].text);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            leagueId = reader.GetInt32(0);
                        }
                    }
                }
            }

            using (var command = connection.CreateCommand())
            {
                int careerTableInt = careerClickedId + 1;
                command.CommandText = $"SELECT country_id FROM leagues WHERE id = @leagueId;";
                command.Parameters.AddWithValue("@leagueId", leagueId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            countryId = reader.GetInt32(0);
                        }
                    }
                }
            }

            using (var command1 = connection.CreateCommand())
            {
                command1.CommandText = "INSERT INTO player (id, firstName, lastName, age, club, club_country_id, club_league_id, teamStat, skillStat, fansMediaStat, financeStat, personalLifeStat, tutoDone) VALUES (@id, @firstName, @lastName, @age, @club, @club_country_id, @club_league_id, @initStat, @initStat, @initStat, @initStat, @initStat, @tutoDone);";
                command1.Parameters.AddWithValue("@id", careerClickedId);
                command1.Parameters.AddWithValue("@firstName", firstNameInputField.text);
                command1.Parameters.AddWithValue("@lastName", lastNameInputField.text);
                command1.Parameters.AddWithValue("@age", 18);
                command1.Parameters.AddWithValue("@club", ClubDisplayDropdown.options[ClubDisplayDropdown.value].text);
                command1.Parameters.AddWithValue("@club_country_id", countryId);
                command1.Parameters.AddWithValue("@club_league_id", leagueId);
                command1.Parameters.AddWithValue("@initStat", 50);
                command1.Parameters.AddWithValue("@tutoDone", "n");

                command1.ExecuteNonQuery();

            }

            using (var command2 = connection.CreateCommand())
            {
                command2.CommandText = "SELECT * FROM player WHERE id = @id;";
                command2.Parameters.AddWithValue("@id", careerClickedId);
                using (var reader = command2.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            bool tutoStatus = false;
                            switch (reader["tutoDone"].ToString())
                            {
                                case "y":
                                    tutoStatus = true;
                                    break;
                                default:
                                case "n":
                                    break;
                            }
                            PlayerManager.Instance.InitPlayer(Convert.ToInt32(reader["id"]), reader["firstName"].ToString(), reader["lastName"].ToString(), Convert.ToInt32(reader["age"]), reader["club"].ToString(), Convert.ToInt32(reader["club_country_id"]), Convert.ToInt32(reader["club_league_id"]),  Convert.ToInt32(reader["teamStat"]), Convert.ToInt32(reader["skillStat"]), Convert.ToInt32(reader["fansMediaStat"]), Convert.ToInt32(reader["financeStat"]), Convert.ToInt32(reader["personalLifeStat"]), tutoStatus);
                        }
                    }
                }
            }
            connection.Close();
        }
    }

    void LoadCareer()
    {
        bool tutoStatus = false;
        using (var connection = new SqliteConnection("URI=file:" + dbName))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM player WHERE id = @id;";
                command.Parameters.AddWithValue("@id", careerClickedId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            tutoStatus = false;
                            switch (reader["tutoDone"].ToString())
                            {
                                case "y":
                                    tutoStatus = true;
                                    break;
                                default:
                                case "n":
                                    break;
                            }
                            PlayerManager.Instance.InitPlayer(Convert.ToInt32(reader["id"]), reader["firstName"].ToString(), reader["lastName"].ToString(), Convert.ToInt32(reader["age"]), reader["club"].ToString(), Convert.ToInt32(reader["club_country_id"]), Convert.ToInt32(reader["club_league_id"]), Convert.ToInt32(reader["teamStat"]), Convert.ToInt32(reader["skillStat"]), Convert.ToInt32(reader["fansMediaStat"]), Convert.ToInt32(reader["financeStat"]), Convert.ToInt32(reader["personalLifeStat"]), tutoStatus);
                        }
                    }
                }
            }
            connection.Close();
        }
        if (tutoStatus)
        {
            loadScene.sceneName = "MainGameScene";
            StartCoroutine(loadScene.LoadSceneAfterFade());
        }
        else
        {
            loadScene.sceneName = "TutoScene";
            StartCoroutine(loadScene.LoadSceneAfterFade());
        }
    }

    public void DeleteCareer()
    {
        using (var connection = new SqliteConnection("URI=file:" + dbName))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.Parameters.AddWithValue("@id", careerClickedId);

                switch (careerClickedId)
                {
                    case 0:
                        command.CommandText = @"
                        DELETE FROM player WHERE id = @id;
                        UPDATE teams_career_1 
                        SET games_played = 0, games_win = 0, games_draw = 0, games_loose = 0, points = 0, gf = 0, ga = 0, gd = 0;";
                        command.ExecuteNonQuery();
                        Debug.Log("Carrière 1 supprimée et équipes réinitialisées !");
                        break;

                    case 1:
                        command.CommandText = @"
                        DELETE FROM player WHERE id = @id;
                        UPDATE teams_career_2 
                        SET games_played = 0, games_win = 0, games_draw = 0, games_loose = 0, points = 0, gf = 0, ga = 0, gd = 0;";
                        command.ExecuteNonQuery();
                        Debug.Log("Carrière 2 supprimée et équipes réinitialisées !");
                        break;

                    case 2:
                        command.CommandText = @"
                        DELETE FROM player WHERE id = @id;
                        UPDATE teams_career_3 
                        SET games_played = 0, games_win = 0, games_draw = 0, games_loose = 0, points = 0, gf = 0, ga = 0, gd = 0;";
                        command.ExecuteNonQuery();
                        Debug.Log("Carrière 3 supprimée et équipes réinitialisées !");
                        break;

                    default:
                        Debug.LogError("Carrière invalide !");
                        break;
                }
            }
            connection.Close();
        }

        DeleteCareerPopUp.SetActive(false);
        UpdateCareerUI();
    }


    public void DisplayInterestedTeams()
    {
        int countryID = 1;
        List<int> leaguesIDs = new List<int>();

        using (var connection = new SqliteConnection("URI=file:" + dbName))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                // Obtenir l'ID du pays
                command.CommandText = "SELECT id FROM countries WHERE name = @name;";
                command.Parameters.AddWithValue("@name", countryInputField.text);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        countryID = reader.GetInt32(0);  // Lire l'ID du pays
                    }
                    reader.Close();
                }

                // Obtenir les IDs des ligues
                command.CommandText = "SELECT id FROM leagues WHERE country_id = @id;";
                command.Parameters.AddWithValue("@id", countryID);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int leagueID = reader.GetInt32(0);  // Lire chaque ID de ligue
                        leaguesIDs.Add(leagueID);
                    }
                    reader.Close();
                }

                // Déterminer la table en fonction de l'ID de carrière
                string tableName;
                switch (careerClickedId)
                {
                    case 0:
                        tableName = "teams_career_1";
                        break;
                    case 1:
                        tableName = "teams_career_2";
                        break;
                    case 2:
                        tableName = "teams_career_3";
                        break;
                    default:
                        tableName = "";
                        break;
                }

                // Vider les options précédentes
                ClubDisplayDropdown.ClearOptions();

                // Récupérer les équipes pour chaque ligue
                foreach (int leagueID in leaguesIDs)
                {
                    command.CommandText = $"SELECT name FROM {tableName} WHERE league_id = @league_id AND coef_force < 60;";
                    command.Parameters.AddWithValue("@league_id", leagueID);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData();
                            optionData.text = reader["name"].ToString();
                            ClubDisplayDropdown.options.Add(optionData);
                        }
                        reader.Close();
                    }
                }

                // Rafraîchir l'affichage du dropdown
                ClubDisplayDropdown.RefreshShownValue();
            }
            connection.Close();


        }
    }

}
