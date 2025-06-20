using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatchManager : MonoBehaviourSingleton<MatchManager>
{
    [Header("PLAYER :")]
    public float _formStat;
    public float _moralStat;
    public float _aggroStat;
    public float _confidenceStat;
    public float _disciplineStat;
    public float _note;
    public bool _isOnPitch;
    public bool _isPlayerHome;

    [Header("STATS :")]
    [SerializeField] public TextMeshProUGUI _goalsScoredText;
    [SerializeField] public TextMeshProUGUI _assistsText;
    [SerializeField] public TextMeshProUGUI _yellowCardText;
    [SerializeField] public TextMeshProUGUI _redCardText;
    [SerializeField] public TextMeshProUGUI _injuryText;

    [Header("FORME :")]
    [SerializeField] Slider _formSlider;

    [Header("SCORE :")]
    [SerializeField] public TextMeshProUGUI _homeTeamName;
    [SerializeField] public int _homeTeamScore;
    [SerializeField] Image _homeColor1;
    [SerializeField] Image _homeColor2;
    [SerializeField] public TextMeshProUGUI _awayTeamName;
    [SerializeField] public int _awayTeamScore;
    [SerializeField] Image _awayColor1;
    [SerializeField] Image _awayColor2;
    [SerializeField] public TextMeshProUGUI _scoreText;

    void Start()
    {
        InitStats();
        InitForm();
        InitScoreCard();
    }

    private void Update()
    {
        _formSlider.value = _formStat;
    }

    void InitStats()
    {
        _formStat = 100;
        _moralStat = 50;
        _aggroStat = 50;
        _confidenceStat = 50;
        _disciplineStat = 50;
        _note = 6;
        _isOnPitch = true;

        _goalsScoredText.text = "0";
        _assistsText.text = "0";
        _yellowCardText.text = "Non";
        _redCardText.text = "Non";
        _injuryText.text = "Non";
    }

    void InitForm()
    {
        _formSlider.value = _formSlider.maxValue;
    }

    void InitScoreCard()
    {
        _homeTeamName.text = MatchDataManager.Instance.currentMatchData._homeTeam;
        _homeColor1.color = HexToRgbColor(MatchDataManager.Instance.currentMatchData._homeTeamColors[0]);
        _homeColor2.color = HexToRgbColor(MatchDataManager.Instance.currentMatchData._homeTeamColors[1]);

        _awayTeamName.text = MatchDataManager.Instance.currentMatchData._awayTeam;
        _awayColor1.color = HexToRgbColor(MatchDataManager.Instance.currentMatchData._awayTeamColors[0]);
        _awayColor2.color = HexToRgbColor(MatchDataManager.Instance.currentMatchData._awayTeamColors[1]);

        _isPlayerHome = MatchDataManager.Instance.currentMatchData._isPlayerHome;
        _homeTeamScore = 0; _awayTeamScore = 0;
        SetMatchScoreText();
    }

    public void SetMatchScoreText()
    {
        _scoreText.text = $"{_homeTeamScore}-{_awayTeamScore}";
    }

    public Color HexToRgbColor(string hexColor)
    {
        Color rgbColor;
        if (ColorUtility.TryParseHtmlString(hexColor, out rgbColor)){ return rgbColor; }
        else { Debug.LogWarning("Invalid HexColor"); return new Color(255f,255f,255f); }
    }
}
