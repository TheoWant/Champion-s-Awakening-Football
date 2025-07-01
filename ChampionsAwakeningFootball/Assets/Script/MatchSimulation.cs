using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class MatchSimulation : MonoBehaviourSingleton<MatchSimulation>
{
    int _matchTime;

    int _homeStrength;
    int _awayStrength;

    List<string> actionSentences;
    List<string> goalSentences;
    List<string> failSentences;

    [Header("Match messages elements")]
    public GameObject _simulationMenu;
    [SerializeField] GameObject _messagePrefab;
    [SerializeField] Transform _contentTransform;
    [SerializeField] ScrollRect _scrollRect;
    public TextMeshProUGUI MatchTime;

    [Header("Player actions elements")]
    [SerializeField] CSVElementLoader csvElementLoader;
    public GameObject _playerMomentMenu;

    [Header("Simulation variables")]
    public GameObject _refereeWhistles;
    public GameObject EndMatchGameObject;
    public bool isSimulating;

    async void Start()
    {
        _matchTime = 1;
        _homeStrength = (int)MatchDataManager.Instance.currentMatchData._homeTeamStrength+5;
        _awayStrength = (int)MatchDataManager.Instance.currentMatchData._awayTeamStrength;

        isSimulating = true;
        MatchTime.text = _matchTime+"'";

        await csvElementLoader.LoadCardsAsync();
        await csvElementLoader.LoadMatchSentencesAsync();

        StartCoroutine(SimulateMatch());
    }

    public void ResimulateMatch()
    {
        StartCoroutine(SimulateMatch());
    }

    IEnumerator SimulateMatch()
    {
        while (_matchTime < 90 && isSimulating)
        {
            if (_matchTime == 45)
            {
                isSimulating = false;
                StartCoroutine(HalfTime());
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
                IncreaseMinuteCounter();
                yield return null;
            }
        }

        if(_matchTime == 90 && isSimulating)
        {
            Instantiate(_refereeWhistles);
            yield return new WaitForSeconds(2.5f);
            EndMatch();
        }

        yield return null;
    }

    ///---------------------------------------------------------------------------------------------------------------------------------------- 

    void IncreaseMinuteCounter()
    {
        _matchTime++;

        int actionProb = Random.Range(0, 101);

        MatchTime.text = _matchTime.ToString()+"'";

        if (actionProb <= 10)
        {
            isSimulating = false;
            StartCoroutine(ActionInMatch());
        }

        if (MatchManager.Instance._isOnPitch)
        {
            MatchManager.Instance._formStat -= Random.Range(0.1f, 0.6f);

            if (MatchManager.Instance._formStat <= 0)
            {
                isSimulating = false;
                _playerMomentMenu.SetActive(true);
                _simulationMenu.SetActive(false);
                CardMatchManager.Instance.InitNewCard(CardMatchManager.Instance.FindCard("REMP"));
                MatchManager.Instance._isOnPitch = false;
                return;
            }

            if (actionProb > 10 && actionProb <= 17)
            {
                isSimulating = false;
                _playerMomentMenu.SetActive(true);
                _simulationMenu.SetActive(false);
                CardMatchManager.Instance.InitNewCard();
            }
        }

    }

    ///----------------------------------------------------------------------------------------------------------------------------------------

    IEnumerator ActionInMatch()
    {
        int teamActionChoice = Random.Range(0, _homeStrength+_awayStrength);

        string teamAction = "";

        if (teamActionChoice > _homeStrength)
        {
            ActionForTeam("home");
            teamAction = "home";
        }
        else
        {
            ActionForTeam("away");
            teamAction = "away";
        }
        yield return null;
        ContentScroll();

        yield return new WaitForSeconds(2f);

        ActionResult(teamAction);
        yield return null;
        ContentScroll();

        yield return new WaitForSeconds(0.7f);

        isSimulating = true;
        StartCoroutine(SimulateMatch());
    }

    ///----------------------------------------------------------------------------------------------------------------------------------------

    void ActionForTeam(string team)
    {
        GameObject message = Instantiate(_messagePrefab, _contentTransform);
        MatchMessage matchMessage = message.GetComponent<MatchMessage>();

        if (team == "home")
        {
            matchMessage._MainMessageText.text = ActionRandomSentence(MatchDataManager.Instance.currentMatchData._homeTeam.ToUpper());
            matchMessage._background.color = MatchManager.Instance.HexToRgbColor(MatchDataManager.Instance.currentMatchData._homeTeamColors[0]);
        }
        else
        {
            matchMessage._MainMessageText.text = ActionRandomSentence(MatchDataManager.Instance.currentMatchData._awayTeam.ToUpper());
            matchMessage._background.color = MatchManager.Instance.HexToRgbColor(MatchDataManager.Instance.currentMatchData._awayTeamColors[0]);
        }

        InitMessagesContrast(matchMessage);
        matchMessage._textTime.text = _matchTime.ToString() + "'";
    }


    ///----------------------------------------------------------------------------------------------------------------------------------------

    void ActionResult(string teamAction)
    {
        int goalChance = Random.Range(0, 350);

        GameObject message = Instantiate(_messagePrefab, _contentTransform);
        MatchMessage matchMessage = message.GetComponent<MatchMessage>();

        switch (teamAction)
        {

            case "home":
                if (goalChance < 2 * _homeStrength - _awayStrength)
                {
                    MatchManager.Instance._homeTeamScore++;
                    MatchManager.Instance.SetMatchScoreText();

                    matchMessage._MainMessageText.text = GoalRandomSentence(MatchDataManager.Instance.currentMatchData._homeTeam.ToUpper(), MatchManager.Instance._scoreText.text);
                }
                else
                {
                    matchMessage._MainMessageText.text = FailRandomSentence(MatchManager.Instance._scoreText.text);
                }
                matchMessage._background.color = MatchManager.Instance.HexToRgbColor(MatchDataManager.Instance.currentMatchData._homeTeamColors[0]);

                break;
            case "away":
                if (goalChance < 2 * _awayStrength - _homeStrength)
                {
                    MatchManager.Instance._awayTeamScore++;
                    MatchManager.Instance.SetMatchScoreText();

                    matchMessage._MainMessageText.text = GoalRandomSentence(MatchDataManager.Instance.currentMatchData._awayTeam.ToUpper(), MatchManager.Instance._scoreText.text);
                }
                else
                {
                    matchMessage._MainMessageText.text = FailRandomSentence(MatchManager.Instance._scoreText.text);
                }
                matchMessage._background.color = MatchManager.Instance.HexToRgbColor(MatchDataManager.Instance.currentMatchData._awayTeamColors[0]);
                break;
            default:
                break;
        }

        InitMessagesContrast(matchMessage);
        matchMessage._textTime.text = _matchTime.ToString() + "'";
    }

    ///----------------------------------------------------------------------------------------------------------------------------------------

    IEnumerator HalfTime()
    {
        GameObject message1 = Instantiate(_messagePrefab, _contentTransform);
        MatchMessage matchMessage1 = message1.GetComponent<MatchMessage>();

        matchMessage1._MainMessageText.text = "-------- MI-TEMPS --------";
        matchMessage1._background.color = Color.gray;
        matchMessage1._background.transform.GetChild(0).gameObject.SetActive(false);
        InitMessagesContrast(matchMessage1);

        yield return null;

        ContentScroll();

        yield return new WaitForSeconds(2f);

        GameObject message2 = Instantiate(_messagePrefab, _contentTransform);
        MatchMessage matchMessage2 = message2.GetComponent<MatchMessage>();

        matchMessage2._MainMessageText.text = "--- DÉBUT DE LA SECONDE PERIODE ---";
        matchMessage2._background.color = Color.gray;
        matchMessage2._background.transform.GetChild(0).gameObject.SetActive(false);
        InitMessagesContrast(matchMessage2);

        yield return null;

        ContentScroll();

        isSimulating = true;
        IncreaseMinuteCounter();
        StartCoroutine(SimulateMatch());

        yield return null;
    }


    void EndMatch()
    {
        isSimulating = false;
        EndMatchGameObject.SetActive(true);
    }

    ///----------------------------------------------------------------------------------------------------------------------------------------


    string ActionRandomSentence(string team)
    {
        string sentence;
        sentence = csvElementLoader.ActionSentences[Random.Range(0, csvElementLoader.ActionSentences.Count)];
        sentence = sentence.Replace("[team]", team);
        return sentence;
    }

    string GoalRandomSentence(string team, string score)
    {
        string sentence;
        sentence = csvElementLoader.GoalSentences[Random.Range(0, csvElementLoader.GoalSentences.Count)];
        sentence = sentence.Replace("[team]", team);
        sentence = sentence.Replace("[score]", score);
        return sentence;
    }

    string FailRandomSentence(string score)
    {
        string sentence;
        sentence = csvElementLoader.FailSentences[Random.Range(0, csvElementLoader.FailSentences.Count)];
        sentence = sentence.Replace("[score]", score);
        return sentence;
    }

    ///----------------------------------------------------------------------------------------------------------------------------------------

    void ContentScroll()
    {
        _scrollRect.verticalNormalizedPosition = 0f; // Scroller automatiquement vers le bas le scroll view pour afficher toujours le message le plus recent
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_contentTransform);
    }

    public static Color GetContrastingTextColor(Color TeamColor)
    {
        float luminance = (0.299f * TeamColor.r + 0.587f * TeamColor.g + 0.114f * TeamColor.b);
        return luminance > 0.7f ? Color.black : Color.white;
    }

    void InitMessagesContrast(MatchMessage message)
    {
        message._MainMessageText.color = GetContrastingTextColor(message._background.color);
        message._messageTimeImage.color = GetContrastingTextColor(message._background.color);
        message._separator.color = GetContrastingTextColor(message._background.color);
        message._textTime.color = GetContrastingTextColor(message._background.color);
    }


}
