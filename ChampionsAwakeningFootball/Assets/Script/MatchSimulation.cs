using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatchSimulation : MonoBehaviourSingleton<MatchSimulation>
{
    int _matchTime;

    int _homeStrength;
    int _awayStrength;

    int _homeScore;
    int _awayScore;

    [SerializeField] GameObject _messagePrefab;
    [SerializeField] Transform _contentTransform;
    [SerializeField] ScrollRect _scrollRect;

    public TextMeshProUGUI MatchTime;

    public GameObject _playerMomentMenu;

    public bool isSimulating;

    private void Start()
    {
        _matchTime = 1;
        _homeStrength = (int)MatchDataManager.Instance.currentMatchData._homeTeamStrength+5;
        _awayStrength = (int)MatchDataManager.Instance.currentMatchData._awayTeamStrength;

        isSimulating = true;
        MatchTime.text = _matchTime+"'";
        StartCoroutine(SimulateMatch());
    }

    public void ResimulateMatch()
    {
        StartCoroutine(SimulateMatch());
    }

    public IEnumerator SimulateMatch()
    {
        while (_matchTime < 90 && isSimulating)
        {
            yield return new WaitForSeconds(0.1f);
            IncreaseMinuteCounter();
            yield return null;
        }
        yield return null;
    }

    void IncreaseMinuteCounter()
    {
        _matchTime++;

        MatchManager.Instance._formStat -= Random.Range(0.1f,0.6f);

        int actionProb = Random.Range(0, 101);

        MatchTime.text = _matchTime.ToString()+"'";

        if (actionProb <= 10)
        {
            isSimulating = false;
            StartCoroutine(ActionInMatch());
        }

        if (MatchManager.Instance._isOnPitch)
        {
            if (MatchManager.Instance._formStat <= 0)
            {
                isSimulating = false;
                _playerMomentMenu.SetActive(true);
                CardMatchManager.Instance.InitNewCard(CardMatchManager.Instance.FindCard("REMP"));
                MatchManager.Instance._isOnPitch = false;
                return;
            }

            if (actionProb > 10 && actionProb <= 16)
            {
                isSimulating = false;
                _playerMomentMenu.SetActive(true);
                CardMatchManager.Instance.InitNewCard();
            }
        }
    }

    IEnumerator ActionInMatch()
    {
        int teamActionChoice = Random.Range(0, _homeStrength+_awayStrength);

        string teamAction = "";

        GameObject message1 = Instantiate(_messagePrefab, _contentTransform);
        MatchMessage matchMessage1 = message1.GetComponent<MatchMessage>();

        if (teamActionChoice > _homeStrength)
        {
            matchMessage1._MainMessageText.text = "Action pour " + MatchDataManager.Instance.currentMatchData._homeTeam.ToUpper();
            matchMessage1._background.color = MatchManager.Instance.HexToRgbColor(MatchDataManager.Instance.currentMatchData._homeTeamColors[0]);
            teamAction = "home";
        }
        else
        {
            matchMessage1._MainMessageText.text = "Action pour " + MatchDataManager.Instance.currentMatchData._awayTeam.ToUpper();
            matchMessage1._background.color = MatchManager.Instance.HexToRgbColor(MatchDataManager.Instance.currentMatchData._awayTeamColors[0]);
            teamAction = "away";
        }

        matchMessage1._MainMessageText.color = GetContrastingTextColor(matchMessage1._background.color);
        matchMessage1._messageTimeImage.color = GetContrastingTextColor(matchMessage1._background.color);
        matchMessage1._separator.color = GetContrastingTextColor(matchMessage1._background.color);
        matchMessage1._textTime.color = GetContrastingTextColor(matchMessage1._background.color);

        matchMessage1._textTime.text = _matchTime.ToString()+"'";

        yield return null;

        _scrollRect.verticalNormalizedPosition = 0f; // Scroller automatiquement vers le bas le scroll view pour afficher toujours le message le plus recent
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_contentTransform);

        yield return new WaitForSeconds(2f);

        int goalChance = Random.Range(0, 350);

        GameObject message2 = Instantiate(_messagePrefab, _contentTransform);
        MatchMessage matchMessage2 = message2.GetComponent<MatchMessage>();

        switch (teamAction)
        {
            
            case "home":
                if (goalChance < 2 * _homeStrength - _awayStrength)
                {
                    _homeScore++;
                    MatchManager.Instance._scoreText.text = $"{_homeScore}-{_awayScore}";
                    matchMessage2._MainMessageText.text = "BUUUUUTTT POUR " + MatchDataManager.Instance.currentMatchData._homeTeam.ToUpper() + " !! Le score est de " + MatchManager.Instance._scoreText.text + " désormais";
                }
                else
                {
                    matchMessage2._MainMessageText.text = "Mais c'est raté...";
                }
                matchMessage2._background.color = MatchManager.Instance.HexToRgbColor(MatchDataManager.Instance.currentMatchData._homeTeamColors[0]);

                break;
            case "away":
                if(goalChance < 2 * _awayStrength - _homeStrength)
                {
                    _awayScore++;
                    MatchManager.Instance._scoreText.text = $"{_homeScore}-{_awayScore}";
                    matchMessage2._MainMessageText.text = "BUUUUUTTT POUR " + MatchDataManager.Instance.currentMatchData._awayTeam.ToUpper()+" !! Le score est de "+ MatchManager.Instance._scoreText.text+" désormais";
                }
                else
                {
                    matchMessage2._MainMessageText.text = "Mais c'est raté...";
                }
                matchMessage2._background.color = MatchManager.Instance.HexToRgbColor(MatchDataManager.Instance.currentMatchData._awayTeamColors[0]);
                break;
            default:
                break;
        }

        matchMessage2._MainMessageText.color = GetContrastingTextColor(matchMessage2._background.color);
        matchMessage2._messageTimeImage.color = GetContrastingTextColor(matchMessage2._background.color);
        matchMessage2._separator.color = GetContrastingTextColor(matchMessage2._background.color);
        matchMessage2._textTime.color = GetContrastingTextColor(matchMessage2._background.color);

        matchMessage2._textTime.text = _matchTime.ToString()+ "'";

        yield return null;

        _scrollRect.verticalNormalizedPosition = 0f; // Scroller automatiquement vers le bas le scroll view pour afficher toujours le message le plus recent
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_contentTransform);

        yield return new WaitForSeconds(0.7f);

        isSimulating = true;
        StartCoroutine(SimulateMatch());

        yield return null;
    }

    public static Color GetContrastingTextColor(Color TeamColor)
    {
        float luminance = (0.299f * TeamColor.r + 0.587f * TeamColor.g + 0.114f * TeamColor.b);
        return luminance > 0.7f ? Color.black : Color.white;
    }
}
