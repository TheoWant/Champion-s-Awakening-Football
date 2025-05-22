using System.IO;
using UnityEngine;
using Mono.Data.Sqlite;
using UnityEngine.UI;
using TMPro.EditorUtilities;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

public class RankingAndCalendarDisplayManager : MonoBehaviour
{
    private string dbName;
    private int CountryID;
    private int LeagueID;

    [SerializeField] private GameObject DisplayPrefab;
    [SerializeField] private Transform DisplayParent;

    [SerializeField] private TMP_Dropdown CountriesDropdown, LeaguesDropdown;

    private Dictionary<string,int> CountryNameAndId = new Dictionary<string,int>();
    private Dictionary<string,int> LeagueNameAndId = new Dictionary<string,int>();

    private List<GameObject> GameObjectDisplayed = new List<GameObject>();

    private LeagueCalendar calendar = new LeagueCalendar();
    private int matchDay;
    public Button prevBtn, nextBtn;
    public TextMeshProUGUI displayMatchDayInt;

    public Fonction _fonction;
    public Filter _filter;


    public enum Fonction
    {
        RANKING,
        CALENDAR
    }

    public enum Filter {
        ALL,
        FIRSTDIV,
        SECONDDIV,
        SMALLCOEFFORCE
    }

    void OnEnable()
    {
        dbName = Path.Combine(Application.persistentDataPath, "Teams.db");

        if(CountriesDropdown && LeaguesDropdown != null)
            CountriesDropdown.onValueChanged.AddListener(delegate { DisplayLeagues(ChangeCountry(), _filter); });
        if(LeaguesDropdown != null)
            LeaguesDropdown.onValueChanged.AddListener(delegate { DisplayTeams(ChangeLeague(), _filter); });

        DisplayCountries(_filter);
        if(LeaguesDropdown != null && CountriesDropdown != null)
        {
            DisplayPlayerLeagueRankingOrCalendar();
        }
    }

    public void DisplayCountries(Filter? filter = Filter.ALL)
    {
        CountriesDropdown.options.Clear();
        using (var connection = new SqliteConnection("URI=file:" + dbName))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                switch (filter)
                {
                    default:
                    case Filter.ALL:
                        command.CommandText = "SELECT * FROM countries ORDER BY name ASC";
                        break;
                    case Filter.FIRSTDIV:
                        break;
                    case Filter.SECONDDIV:
                        break;
                }
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData();
                        optionData.text = reader["name"].ToString();
                        if (!CountryNameAndId.ContainsKey(reader["name"].ToString()))
                        {
                            CountryNameAndId.Add(reader["name"].ToString(), int.Parse(reader["id"].ToString()));
                        }
                        CountriesDropdown.options.Add(optionData);
                    }
                    reader.Close();
                }
            }

            CountriesDropdown.value = 0;
            CountriesDropdown.RefreshShownValue();

            if (LeaguesDropdown != null)
            {
                DisplayLeagues(ChangeCountry(), filter);
            }
            connection.Close();
        }
    }

    public void DisplayLeagues(KeyValuePair<string,int> countryID, Filter? filter = Filter.ALL)
    {
        LeagueNameAndId.Clear();

        LeaguesDropdown.options.Clear();
        using (var connection = new SqliteConnection("URI=file:" + dbName))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                switch (filter)
                {
                    case Filter.FIRSTDIV:
                        command.CommandText = "SELECT * FROM leagues WHERE country_id = @id AND id % 2 <> 0;";
                        command.Parameters.AddWithValue("@id", countryID.Value);
                        break;
                    case Filter.SECONDDIV:
                        command.CommandText = "SELECT * FROM leagues WHERE country_id = @id AND id % 2 = 0;";
                        command.Parameters.AddWithValue("@id", countryID.Value);
                        break;
                    default:
                    case Filter.ALL:
                        command.CommandText = "SELECT * FROM leagues WHERE country_id = @id;";
                        command.Parameters.AddWithValue("@id", countryID.Value);
                        break;
                }
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData();
                        optionData.text = reader["name"].ToString();
                        if (!LeagueNameAndId.ContainsKey(reader["name"].ToString()))
                        {
                            LeagueNameAndId.Add(reader["name"].ToString(), int.Parse(reader["id"].ToString()));
                        }
                        LeaguesDropdown.options.Add(optionData);
                    }
                    reader.Close();
                }
            }
            connection.Close();

            LeaguesDropdown.value = 0;
            LeaguesDropdown.captionText.text = LeaguesDropdown.options[LeaguesDropdown.value].text;

            if(DisplayParent != null && DisplayPrefab != null)
            {
                DisplayTeams(ChangeLeague(), filter);
            }
        }
    }

    public void DisplayTeams(KeyValuePair<string, int> leagueID, Filter? filter = Filter.ALL)
    {
        foreach (GameObject team in GameObjectDisplayed)
        {
            Destroy(team);
        }
        GameObjectDisplayed.Clear();
        string table = "";
        switch (PlayerManager.Instance._id)
        {
            case 0:
                table = "teams_career_1";
                break;
            case 1:
                table = "teams_career_2";
                break;
            case 2:
                table = "teams_career_3";
                break;
        }

        switch (_fonction)
        {
            case Fonction.RANKING:
            default:
                using (var connection = new SqliteConnection("URI=file:" + dbName))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        switch (filter)
                        {
                            case Filter.SMALLCOEFFORCE:
                                command.CommandText = $"SELECT * FROM {table} WHERE league_id = @id AND coef_force <= 65 ORDER BY points DESC;";
                                command.Parameters.AddWithValue("@id", leagueID.Value);
                                break;
                            default:
                            case Filter.ALL:
                                command.CommandText = $"SELECT * FROM {table} WHERE league_id = @id ORDER BY points DESC;";
                                command.Parameters.AddWithValue("@id", leagueID.Value);
                                break;
                        }
                        command.CommandText = $"SELECT * FROM {table} WHERE league_id = @id ORDER BY points DESC;";
                        command.Parameters.AddWithValue("@id", leagueID.Value);

                        using (var reader = command.ExecuteReader())
                        {
                            int i = 1;
                            while (reader.Read())
                            {
                                GameObject team = Instantiate(DisplayPrefab, DisplayParent);
                                ScoreboardTeamInfo sti = team.GetComponent<ScoreboardTeamInfo>();

                                sti.teamRankText.text = i.ToString() + ".";
                                sti.teamNameText.text = reader["name"].ToString();
                                sti.teamPlayedGamesText.text = reader["games_played"].ToString();
                                sti.teamWinCountText.text = reader["games_win"].ToString();
                                sti.teamDrawCountText.text = reader["games_draw"].ToString();
                                sti.teamDefeatCountText.text = reader["games_loose"].ToString();
                                sti.teamGoalAvgText.text = reader["gd"].ToString();
                                sti.teamPointsText.text = reader["points"].ToString();

                                GameObjectDisplayed.Add(team);
                                i++;
                            }
                            reader.Close();
                        }
                    }
                    connection.Close();
                }
                break;
            case Fonction.CALENDAR:
                string folderPath = Path.Combine(Application.persistentDataPath, "Calendars");
                string leagueName = "";

                using (var connection = new SqliteConnection("URI=file:" + dbName))
                {
                    
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = $"SELECT name FROM leagues WHERE id = @id;";
                        command.Parameters.AddWithValue("@id", leagueID.Value);

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

                Debug.Log(leagueName);

                if (Directory.Exists(folderPath) && Directory.GetFiles(folderPath).Count() > 0)
                {
                    Debug.Log(folderPath + leagueName + ".xml");
                    if (SaveManagement.Instance.Read(out SaveObject saveObject, folderPath+"/"+leagueName+".xml"))
                    {
                        calendar = saveObject.calendarData;
                        Debug.Log(calendar.LeagueName.ToString());
                    }
                }

                matchDay = 1;

                DisplayCalendarMatchDays();

                break;
        }
    }

    void DisplayCalendarMatchDays()
    {
        displayMatchDayInt.text = $"Journée {matchDay}";

        if (matchDay == 1) { prevBtn.interactable = false; }
        else if (matchDay == calendar.MatchDays.Count()) { nextBtn.interactable = false; }
        else { prevBtn.interactable = true; nextBtn.interactable = true; }

        List<CalendarMatchData> matchsDayToDisplay = calendar.MatchDays[matchDay-1];

        foreach (GameObject matchObject in GameObjectDisplayed)
        {
            Destroy(matchObject);
        }

        foreach (var match in matchsDayToDisplay)
        {
            GameObject matchDisplay = Instantiate(DisplayPrefab, DisplayParent);
            CalendarMatchInfo cmi = matchDisplay.GetComponent<CalendarMatchInfo>();

            cmi.team1NameText.text = match.HomeTeam;
            cmi.team2NameText.text = match.AwayTeam;
            if (match.IsPlayed)
                cmi.scoreText.text = $"{match.HomeScore}-{match.AwayScore}";
            else
                cmi.scoreText.text = "VS";

            GameObjectDisplayed.Add(matchDisplay);
        }
    }

    public void ChangeMatchDay(bool prev)
    {
        if (prev) { matchDay--; }
        else { matchDay++; }
        DisplayCalendarMatchDays();
    }

    void DisplayPlayerLeagueRankingOrCalendar()
    {
        string countryName = "", leagueName = "";

        using (var connection = new SqliteConnection("URI=file:" + dbName))
        {


            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT name FROM countries WHERE id = @country_id";
                command.Parameters.AddWithValue("@country_id", PlayerManager.Instance._clubCountryId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        countryName = reader.GetString(0);
                    }
                    reader.Close();
                }
            }

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT name FROM leagues WHERE id = @league_id";
                command.Parameters.AddWithValue("@league_id", PlayerManager.Instance._clubLeagueId);

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

        if(CountriesDropdown != null)
        {

        }
        for (int i = 0; i < CountriesDropdown.options.Count; i++)
        {

            if (CountriesDropdown.options[i].text == countryName)
            {
                CountriesDropdown.value = i;
                break;
            }
        }
        for (int i = 0; i < LeaguesDropdown.options.Count; i++)
        {

            if (LeaguesDropdown.options[i].text == leagueName)
            {
                LeaguesDropdown.value = i;
                break;
            }
        }
    }

    public KeyValuePair<string,int> ChangeCountry()
    {
        if (CountryNameAndId.TryGetValue(CountriesDropdown.options[CountriesDropdown.value].text, out int id))
            return new KeyValuePair<string, int>(CountriesDropdown.options[CountriesDropdown.value].text, id);
        else
            return default;
    }

    public KeyValuePair<string, int> ChangeLeague()
    {
        if (LeagueNameAndId.TryGetValue(LeaguesDropdown.options[LeaguesDropdown.value].text, out int id))
            return new KeyValuePair<string, int>(LeaguesDropdown.options[LeaguesDropdown.value].text, id);
        else
            return default;
    }
}
