using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Xml.Linq;
using System.Xml.Serialization;
using UnityEditor.U2D.Aseprite;

[Serializable]
public class CalendarMatchData
{
    public string HomeTeam;
    public string AwayTeam;
    public int? HomeScore; // null si pas encore joué
    public int? AwayScore;
    public bool IsPlayed => HomeScore.HasValue && AwayScore.HasValue;
}

[Serializable]
public class LeagueCalendar
{
    public string LeagueName;
    public int CurrentMatchDay = 0;
    public List<List<CalendarMatchData>> MatchDays; // Chaque élément = 1 journée
}

public static class CalendarGenerator
{
    public static LeagueCalendar GenerateDoubleRoundRobin(List<string> teams)
    {
        if (teams.Count % 2 != 0)
        {
            return null;
        }

        int numTeams = teams.Count;
        int numRounds = numTeams - 1;
        int matchesPerRound = numTeams / 2;

        LeagueCalendar fullCalendar = new LeagueCalendar() { MatchDays = new List<List<CalendarMatchData>>() };

        List<string> rotation = new List<string>(teams);

        for (int round = 0; round < numRounds; round++)
        {
            List<CalendarMatchData> matchDay = new List<CalendarMatchData>();

            for (int match = 0; match < matchesPerRound; match++)
            {
                string team1 = rotation[match];
                string team2 = rotation[numTeams - 1 - match];

                CalendarMatchData matchData;

                if (round % 2 == 0)
                {
                    matchData = new CalendarMatchData
                    {
                        HomeTeam = team1,
                        AwayTeam = team2
                    };
                }
                else
                {
                    matchData = new CalendarMatchData
                    {
                        HomeTeam = team2,
                        AwayTeam = team1
                    };
                }
                
                matchDay.Add(matchData);
            }

            fullCalendar.MatchDays.Add(matchDay);

            // Rotation : la première équipe reste fixe, les autres tournent
            string last = rotation[numTeams - 1];
            rotation.RemoveAt(numTeams - 1);
            rotation.Insert(1, last);
        }

        // Ajouter les matchs retour en inversant les domiciles
        List<List<CalendarMatchData>> returnCalendar = fullCalendar.MatchDays.Select(round => round.Select(match => new CalendarMatchData
        {
            HomeTeam = match.AwayTeam,
            AwayTeam = match.HomeTeam
        }).ToList()).ToList();

        fullCalendar.MatchDays.AddRange(returnCalendar);

        return fullCalendar;
    }

    public static List<LeagueCalendar> GenerateAllLeaguesCalendar(string db)//Lien vers la base de données
    {

        List<LeagueCalendar> AllCalendars = new List<LeagueCalendar>();

        using (var connection = new SqliteConnection("URI=file:" + db))
        {
            connection.Open();

            using (var command1 = connection.CreateCommand())
            {
                command1.CommandText = "SELECT name, id FROM leagues;";
                using (var reader1 = command1.ExecuteReader())
                {
                    while (reader1.Read())
                    {
                        string leagueName = reader1.GetString(0);
                        int leagueId = reader1.GetInt32(1);
                        List<string> leagueTeams = new List<string>();

                        using (var command2 = connection.CreateCommand())
                        {
                            int careerId = PlayerManager.Instance._id + 1;
                            command2.CommandText = $"SELECT name FROM teams_career_{careerId.ToString()} WHERE league_id = @leagueId;";
                            command2.Parameters.AddWithValue("@leagueId", leagueId);

                            using (var reader2 = command2.ExecuteReader())
                            {
                                while (reader2.Read())
                                {
                                    string teamName = reader2.GetString(0);
                                    leagueTeams.Add(teamName);
                                }
                            }
                        }
                        List<string> shuffledList = Shuffle(leagueTeams);
                        var leagueCalendar = GenerateDoubleRoundRobin(shuffledList);
                        leagueCalendar.LeagueName = leagueName;
                        AllCalendars.Add(leagueCalendar);
                    }
                }
            }

        }

        return AllCalendars;
    }


    public static List<T> Shuffle<T>(this List<T> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
        return ts;
    }
}

public class DisplayCalendar : MonoBehaviour
{
    private static string dbName;
    private static string folderPath;
    SaveObject _save;

    void Awake()
    {
        dbName = Path.Combine(Application.persistentDataPath, "Teams.db");
        folderPath = Path.Combine(Application.persistentDataPath, "Calendars");
        _save = new SaveObject();
    }


    private void Start()
    {
        GenerateLeaguesCalendar();
    }

    private void GenerateLeaguesCalendar()
    {
        List<LeagueCalendar> AllLeagueCalendar = new List<LeagueCalendar>();

        if (Directory.Exists(folderPath) && Directory.GetFiles(folderPath).Count() > 0)
        {
            Debug.Log("------------ READ ------------");
            foreach (string f in Directory.GetFiles(folderPath))
            {
                if (SaveManagement.Instance.Read(out SaveObject saveObject, f))
                {
                    AllLeagueCalendar.Add(saveObject.calendarData);
                }
            }
        }
        else
        {
            Debug.Log("------------ WRITE ------------");
            AllLeagueCalendar = CalendarGenerator.GenerateAllLeaguesCalendar(dbName);

            for (int j = 0; j < AllLeagueCalendar.Count; j++)
            {
                var calendar = AllLeagueCalendar[j];

                _save.calendarData = calendar;

                SaveManagement.Write(_save, SaveManagement.FilePath + $"/Calendars/{calendar.LeagueName}.xml");
            }
        }

        
        string debugText = "";

        for (int j = 0; j < AllLeagueCalendar.Count; j++)
        {
            var calendar = AllLeagueCalendar[j];
            debugText += $"--- LEAGUE : {calendar.LeagueName} ---\n\n";
            for (int i = 0; i < calendar.MatchDays.Count; i++)
            {
                debugText += $"--- Journée {i + 1} ---\n\n";
                foreach (var match in calendar.MatchDays[i])
                {
                    if (!match.IsPlayed)
                        debugText += $"{match.HomeTeam} vs {match.AwayTeam} \n";
                    else
                        debugText += $"{match.HomeTeam+" "+match.HomeScore} - {match.AwayScore+" "+match.AwayTeam} \n";
                }
                debugText += $"\n\n\n";
            }
        }

        Debug.Log(debugText);
    }
}
