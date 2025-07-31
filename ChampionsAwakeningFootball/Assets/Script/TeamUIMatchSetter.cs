using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class TeamUIMatchSetter : MonoBehaviour
{

    [SerializeField] public TextMeshProUGUI _homeTeamName;
    [SerializeField] public TextMeshProUGUI _awayTeamName;
    [SerializeField] Image _homeColor1;
    [SerializeField] Image _homeColor2;
    [SerializeField] Image _awayColor1;
    [SerializeField] Image _awayColor2;

    private void Start()
    {
        InitScoreCard();
    }

    void InitScoreCard()
    {
        _homeTeamName.text = MatchDataManager.Instance.currentMatchData._homeTeam;
        _homeColor1.color = MatchManager.Instance.HexToRgbColor(MatchDataManager.Instance.currentMatchData._homeTeamColors[0]);
        _homeColor2.color = MatchManager.Instance.HexToRgbColor(MatchDataManager.Instance.currentMatchData._homeTeamColors[1]);

        _awayTeamName.text = MatchDataManager.Instance.currentMatchData._awayTeam;
        _awayColor1.color = MatchManager.Instance.HexToRgbColor(MatchDataManager.Instance.currentMatchData._awayTeamColors[0]);
        _awayColor2.color = MatchManager.Instance.HexToRgbColor(MatchDataManager.Instance.currentMatchData._awayTeamColors[1]);
    }

}
