using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MatchData
{
    [Header("Data for setting up | Init before a match")]
    public string _homeTeam;
    public string _awayTeam;
    public float _homeTeamStrength;
    public float _awayTeamStrength;
    public string[] _homeTeamColors;
    public string[] _awayTeamColors;
    public string _playerClub;
    public bool _isPlayerHome;

    [Header("Data for getting match results | Init after a match")]
    public int _homeScore;
    public int _awayScore;
    public float _playerRating;
    public bool _wasInjured;
    public bool _gotYellowCard;
    public bool _gotRedCard;
}


public class MatchDataManager : MonoBehaviourSingletonPersistent<MatchDataManager>
{
    public MatchData currentMatchData;

    public void SetUpNewMatch(string home, string away, float homeStr, float awayStr, string playerClub, string homeColor1, string homeColor2, string awayColor1, string awayColor2)
    {
        currentMatchData = new MatchData
        {
            _homeTeam = home,
            _awayTeam = away,
            _homeTeamStrength = homeStr,
            _awayTeamStrength = awayStr,
            _homeTeamColors = new string[2] { homeColor1 , homeColor2 },
            _awayTeamColors = new string[2] { awayColor1, awayColor2 },
            _playerClub = playerClub,
            _isPlayerHome = (playerClub == home)
        };
    }

    public void StoreMatchResult(int homeScore, int awayScore, float rating, bool injured, bool yellow, bool red)
    {
        currentMatchData._homeScore = homeScore;
        currentMatchData._awayScore = awayScore;
        currentMatchData._playerRating = rating;
        currentMatchData._wasInjured = injured;
        currentMatchData._gotYellowCard = yellow;
        currentMatchData._gotRedCard = red;
    }
}
