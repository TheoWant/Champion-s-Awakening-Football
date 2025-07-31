using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayEndMatch : MonoBehaviour
{
    [Header("--- Result ---")]
    [Header("End match images")]
    [SerializeField] Image topImage;
    [SerializeField] Image bottomImage;

    [Header("Sprites to display")]
    [Header("Top sprites")]
    [SerializeField] Sprite v_top_sprite;
    [SerializeField] Sprite n_top_sprite;
    [SerializeField] Sprite d_top_sprite;

    [Header("Bottom sprites")]
    [SerializeField] Sprite v_bottom_sprite;
    [SerializeField] Sprite n_bottom_sprite;
    [SerializeField] Sprite d_bottom_sprite;

    [Header("--- Player Match Recap ---")]
    [Header("Stats texts to fill")]
    [SerializeField] TextMeshProUGUI _moralStatTMP;
    [SerializeField] TextMeshProUGUI _aggressivityStatTMP;
    [SerializeField] TextMeshProUGUI _confidenceStatTMP;
    [SerializeField] TextMeshProUGUI _disciplineStatTMP;
    [SerializeField] TextMeshProUGUI _finalNoteStatTMP;

    void Start()
    {
        MatchManager mM = MatchManager.Instance;
        if (mM._isPlayerHome && mM._homeTeamScore > mM._awayTeamScore || !mM._isPlayerHome && mM._awayTeamScore > mM._homeTeamScore)
        {
            topImage.sprite = v_top_sprite;
            bottomImage.sprite = v_bottom_sprite;
        }
        else if (mM._awayTeamScore == mM._homeTeamScore)
        {
            topImage.sprite = n_top_sprite;
            bottomImage.sprite = n_bottom_sprite;
        }
        else
        {
            topImage.sprite = d_top_sprite;
            bottomImage.sprite = d_bottom_sprite;
        }

        _moralStatTMP.text = mM._moralStat.ToString();
        _aggressivityStatTMP.text = mM._aggroStat.ToString();
        _confidenceStatTMP.text = mM._confidenceStat.ToString();
        _disciplineStatTMP.text = mM._disciplineStat.ToString();
        float finalScoreFloat = (mM._note + (((50f - mM._moralStat) + (50f - mM._aggroStat) + (50f - mM._confidenceStat) + (50f - mM._disciplineStat)) / 10)) / 10;
        double finalNote = finalScoreFloat > 10 ? 10 : (Math.Round(finalScoreFloat, 1, MidpointRounding.ToEven)); // Round note to first decimal OR Round to 10 if upper.
        _finalNoteStatTMP.text = finalNote.ToString();
    }

    void Update()
    {
        
    }
}
