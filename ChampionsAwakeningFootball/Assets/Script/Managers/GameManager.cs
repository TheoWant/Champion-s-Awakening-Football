using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviourSingletonPersistent<GameManager>
{
    private string _dbName;
    [SerializeField] private int _careerNumber;
    [SerializeField] private int _actualmatchDay;

    [SerializeField] private string _playerTeamName;
    [SerializeField] private string _playerLeagueName;
    [SerializeField] private string _playerCountryOfLeagueName;

    
    public string DbName { get { return _dbName; } }
    public int CareerNumber { get { return _careerNumber; } set { _careerNumber = value; } }
    public int ActualMatchDay { get { return _actualmatchDay; } set { _actualmatchDay = value; } }
    public string PlayerTeamName { get { return _playerTeamName; } set { _playerTeamName = value; } }
    public string PlayerLeagueName { get { return _playerLeagueName; } set { _playerLeagueName = value; } }
    public string PlayerCountryOfLeagueName { get { return _playerCountryOfLeagueName; } set { _playerCountryOfLeagueName = value; } }


    public override void Awake()
    {
        base.Awake();
        _dbName = Path.Combine(Application.persistentDataPath, "Teams.db");
        _actualmatchDay = 1; 
    }

    public void SetPlayerLeagueNameFromID(int leagueID)
    {
        string leagueName = "";
        using (var connection = new SqliteConnection("URI=file:" + DbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"SELECT name FROM leagues WHERE id = @id;";
                command.Parameters.AddWithValue("@id", leagueID);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        leagueName = reader.GetString(0);
                    }
                    reader.Close();
                }
            }
            connection.Close();
        }
        PlayerLeagueName = leagueName;
    }

    public void SetPlayerCountryFromID(int countryId)
    {
        string countryName = "";
        using (var connection = new SqliteConnection("URI=file:" + DbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"SELECT name FROM countries WHERE id = @id;";
                command.Parameters.AddWithValue("@id", countryId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        countryName = reader.GetString(0);
                    }
                    reader.Close();
                }
            }
            connection.Close();
        }
        PlayerCountryOfLeagueName = countryName;
    }
}
