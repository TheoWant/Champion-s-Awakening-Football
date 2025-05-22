using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CardMatchManager : MonoBehaviourSingleton<CardMatchManager>
{
    [SerializeField] GameObject _cardPrefab;
    [SerializeField] public Transform _cardContainer;


    public List<CardMatch> _classicMatchCard;
    public List<CardMatch> _specialMatchCard;

    private void Awake()
    {
        LoadCards();
    }

    void LoadCards()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "CardMatch.xml");

        if (File.Exists(filePath))
        {
            if(SaveManagement.Instance.Read(out SaveObject saveObject, filePath))
            {
                foreach (CardMatch card in saveObject.cardMatchList)
                {
                    if (card._id.Substring(0, 2) == "EM")
                        _classicMatchCard.Add(card);
                    else
                        _specialMatchCard.Add(card);
                }
            }
        }
    }

    public void InitNewCard(CardMatch? cm = null)
    {
        GameObject newCard = Instantiate(_cardPrefab, _cardContainer);

        CardDatas cd = newCard.GetComponent<CardDatas>();
        if (_classicMatchCard.Count == 0) { return; }

        if(cm ==  null)
            cm = _classicMatchCard[Random.Range(0, (_classicMatchCard.Count))];

        Debug.Log(cm._text);

        cd._message = cm._text;
        cd._answer1 = cm._choice_A;
        cd._answer2 = cm._choice_B;

        SetupImpact(cd.answer1Text.GetComponent<AnswerImpact>(), cm._impactC1, cm.nextCard_ChoiceA_1, cm.nextCard_ChoiceA_2, cm.proba_nC_CA1);
        SetupImpact(cd.answer2Text.GetComponent<AnswerImpact>(), cm._impactC2, cm.nextCard_ChoiceB_1, cm.nextCard_ChoiceB_2, cm.proba_nC_CB1);

    }

    string RandomNextCard(string id1, string id2, int probaId1)
    {
        int random = Random.Range(0, 101);
        if (random < probaId1)
            return id1;
        else return id2;
    }

    public CardMatch FindCard(string id)
    {
        foreach (CardMatch card in _specialMatchCard) {
            if (card._id == id)
            {
                return card;
            }
        }
        return null;
    }

    void SetupImpact(AnswerImpact ai, int[] impacts, string id1, string id2, int proba)
    {
        ai._formStatImpact = impacts[0];
        ai._moralStatImpact = impacts[1];
        ai._aggroStatImpact = impacts[2];
        ai._confidenceStatImpact = impacts[3];
        ai._disciplineStatImpact = impacts[4];
        ai._noteImpact = impacts[5];
        ai._nextCard = FindCard(RandomNextCard(id1, id2, proba));
    }
}
