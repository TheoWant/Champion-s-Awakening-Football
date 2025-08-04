using Mono.Data.Sqlite;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CardEventManager : MonoBehaviourSingletonPersistent<CardEventManager>
{
    public int eventBeforeMatch;
    string folderPath;
    LeagueCalendar calendar;

    struct MatchDatas
    {
        public int homeStr;
        public string color1;
        public string color2;
    }

    public override void Awake()
    {
        base.Awake();
        folderPath = Path.Combine(Application.persistentDataPath, "Calendars");
    }

    void SetEventBeforeMatchCount()
    {
        eventBeforeMatch = 5;
    }

    public void MatchCardEvent(AnswerImpact selectedAnswerImpact, GameObject swipedCard)
    {
        // If there is a card linked to this choice and proc by the random, then we instantiate this card

        if (selectedAnswerImpact._nextCard != null)
        {
            CardManager.Instance.InitNewCard(selectedAnswerImpact._nextCard);
                
            Destroy(swipedCard);
        }

        // Else we start the simulation back

        else
        {
            foreach (Transform child in CardManager.Instance._cardContainer)
            {
                Destroy(child.gameObject);
            }

            MatchSimulation.Instance.isSimulating = true;
            MatchSimulation.Instance.ResimulateMatch();
            MatchSimulation.Instance._simulationMenu.SetActive(true);
            CardManager.Instance.gameObject.SetActive(false);

        }
    }

    public void MainGameEvent(AnswerImpact selectedAnswerImpact, GameObject swipedCard)
    {
        if (!string.IsNullOrWhiteSpace(selectedAnswerImpact._nextCard._id))
        {
            Debug.Log("PAS NULL");
            CardManager.Instance.InitNewCard(selectedAnswerImpact._nextCard);
        }
        else
        {
            eventBeforeMatch--;
            if (eventBeforeMatch != 0) CardManager.Instance.InitNewCard();
            else { GoMatch(); }
        }
        Destroy(swipedCard);
    }

    void GoMatch()
    {
        SetUpMatch();
        SceneManager.LoadScene("MatchScene");
    }


    void SetUpMatch()
    {

        if (Directory.Exists(folderPath) && Directory.GetFiles(folderPath).Count() > 0)
        {
            if (SaveManagement.Instance.Read(out SaveObject saveObject, folderPath + "/" + GameManager.Instance.PlayerLeagueName + ".xml"))
            {
                calendar = saveObject.calendarData;
                Debug.Log(calendar.LeagueName.ToString());
            }
        }

        float playerForm = 0;
        string home = "";
        string away = "";
        string playerClub = "";

        foreach (CalendarMatchData match in calendar.MatchDays[GameManager.Instance.ActualMatchDay])
        {
            Debug.Log(match.HomeTeam + " VS " + match.AwayTeam + GameManager.Instance.PlayerTeamName);
            if (match.HomeTeam == GameManager.Instance.PlayerTeamName || match.AwayTeam == GameManager.Instance.PlayerTeamName)
            {
                home = match.HomeTeam;
                away = match.AwayTeam;

                playerClub = match.HomeTeam == GameManager.Instance.PlayerTeamName ? match.HomeTeam : match.AwayTeam;

                Debug.LogWarning("MATCH DU JOUEUR = "+home + " VS " + away);
            }
        }

        MatchDatas[] mD = GetMatchDatas(home, away);

        float homeStr = mD[0].homeStr;
        float awayStr = mD[1].homeStr;
        string homeColor1 = mD[0].color1;
        string homeColor2 = mD[0].color2;
        string awayColor1 = mD[1].color1;
        string awayColor2 = mD[1].color2;

        MatchDataManager.Instance.SetUpNewMatch(playerForm, home, away, homeStr, awayStr, playerClub, homeColor1, homeColor2, awayColor1, awayColor2);
    }

    MatchDatas[] GetMatchDatas(string homeName, string awayName)
    {
        MatchDatas home = new MatchDatas();
        MatchDatas away = new MatchDatas();

        using (var connection = new SqliteConnection("URI=file:" + GameManager.Instance.DbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"SELECT coef_force, Color_hex_1, Color_hex_2 FROM teams_career_{GameManager.Instance.CareerNumber} WHERE name = @HomeName OR name = @AwayName;";
                command.Parameters.AddWithValue("@HomeName", homeName);
                command.Parameters.AddWithValue("@AwayName", awayName);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        int row = 0;
                        while (reader.Read())
                        {
                            string color2 = reader.IsDBNull(2) ? null : reader.GetString(2);

                            if (row == 0)
                            {
                                home.homeStr = reader.GetInt32(0);
                                home.color1 = reader.IsDBNull(1) ? null : reader.GetString(1);
                                home.color2 = reader.IsDBNull(2) ? null : reader.GetString(2);
                            }
                            else if (row == 1)
                            {
                                away.homeStr = reader.GetInt32(0);
                                away.color1 = reader.IsDBNull(1) ? null : reader.GetString(1);
                                away.color2 = reader.IsDBNull(2) ? null : reader.GetString(2);
                            }
                            row++;
                        }
                    }
                }
            }
            connection.Close();
        }

        return new MatchDatas[] { home, away };
    }
    
}
