using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardDisplayer : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI _speakerText;
    [SerializeField] public TextMeshProUGUI _messageText;
    [SerializeField] public Transform _cardContainer;

    private void Start()
    {
        RefreshCardDisplay();
    }

    public void RefreshCardDisplay()
    {
        StartCoroutine(RefreshCardDisplayC());
    }

    // Update is called once per frame
    public IEnumerator RefreshCardDisplayC()
    {
        yield return null;
        _speakerText.text = _cardContainer.GetChild(0).GetComponent<CardDatas>()._speaker;
        _messageText.text = _cardContainer.GetChild(0).GetComponent<CardDatas>()._message;
    }
}
