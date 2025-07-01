using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayEndMatch : MonoBehaviour
{
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
            /*topImage.sprite = n_top_sprite;
            bottomImage.sprite = n_bottom_sprite;*/     // TODO IMAGES
        }
        else
        {
            topImage.sprite = d_top_sprite;
            bottomImage.sprite = d_bottom_sprite;
        }
    }

    void Update()
    {
        
    }
}
