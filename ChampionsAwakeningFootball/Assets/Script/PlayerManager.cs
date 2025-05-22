using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Mono.Data.Sqlite;
using System.IO;

public class PlayerManager : MonoBehaviourSingletonPersistent<PlayerManager>
{
    public int _id;
    public string _firstName;
    public string _lastName;
    public int _age;
    public string _club;
    public int _clubCountryId;
    public int _clubLeagueId;

    public int _teamStatValue;
    public int _skillStatValue;
    public int _fansMediaStatValue;
    public int _financeStatValue;
    public int _personalLifeStatValue;

    public bool _isTutoDone;

    private string dbName;

    private void Start()
    {
        dbName = Path.Combine(Application.persistentDataPath, "Teams.db");
    }

    public void InitPlayer(int id, string firstName, string lastName, int age, string club, int club_country_id, int club_league_id, int teamStat, int skillStat, int fansMediaStat, int financeStat, int personalLifeStat, bool isTutoDone)
    {
        _id = id;
        _firstName = firstName;
        _lastName = lastName;
        _age = age;
        _club = club;
        _clubCountryId = club_country_id;
        _clubLeagueId = club_league_id;
        _teamStatValue = teamStat;
        _skillStatValue = skillStat;
        _fansMediaStatValue = fansMediaStat;
        _financeStatValue = financeStat;
        _personalLifeStatValue = personalLifeStat;
        _isTutoDone = isTutoDone;
    }

    public void UpdatePlayerInDatabase()
    {
        using (var connection = new SqliteConnection("URI=file:" + dbName))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE player SET age=@age, club=@club, club_country_id = @c_c_i, club_league_id = @c_l_i, teamStat=@teamStat, skillStat=@skillStat, fansMediaStat=@fansMediaStat, financeStat=@financeStat, personalLifeStat=@personalLifeStat, tutoDone=@tutoDone WHERE id = @id;";
                command.Parameters.AddWithValue("@id", _id);
                command.Parameters.AddWithValue("@age", _age);
                command.Parameters.AddWithValue("@club", _club);
                command.Parameters.AddWithValue("@c_c_i", _clubCountryId);
                command.Parameters.AddWithValue("@c_l_i", _clubLeagueId);
                command.Parameters.AddWithValue("@teamStat", _teamStatValue);
                command.Parameters.AddWithValue("@skillStat", _skillStatValue);
                command.Parameters.AddWithValue("@fansMediaStat", _fansMediaStatValue);
                command.Parameters.AddWithValue("@financeStat", _financeStatValue);
                command.Parameters.AddWithValue("@personalLifeStat", _personalLifeStatValue);
                command.Parameters.AddWithValue("@tutoDone", _isTutoDone? "y" : "n");

                command.ExecuteNonQuery();
            }
            connection.Close();
        }
    }
}
