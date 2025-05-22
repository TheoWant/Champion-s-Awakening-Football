using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDatas : MonoBehaviour
{
    [SerializeField] public string _speaker;
    [SerializeField] public string _message;
    [SerializeField] public string _answer1;
    [SerializeField] public string _answer2;
    [SerializeField] public Sprite _cardImage;

    [SerializeField] public TextMeshProUGUI answer1Text;
    [SerializeField] public TextMeshProUGUI answer2Text;

    private void Start()
    {
        answer1Text.text = _answer1;
        answer2Text.text = _answer2;
    }
}
