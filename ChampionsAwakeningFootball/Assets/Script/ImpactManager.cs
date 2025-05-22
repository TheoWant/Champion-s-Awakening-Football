using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ImpactManager : MonoBehaviourSingleton<ImpactManager>
{
    public enum State
    {
        MAINGAME,
        MATCH
    }

    [SerializeField] public Image _teamFilledImage;
    [SerializeField] public Image _skillFilledImage;
    [SerializeField] public Image _fansAndMediaFilledImage;
    [SerializeField] public Image _financesFilledImage;
    [SerializeField] public Image _personalLifeFilledImage;

    public State state;

    public void ApplyImpact(AnswerImpact answerImpact)
    {
        switch (state)
        {
            case State.MAINGAME:
                _teamFilledImage.fillAmount += answerImpact._teamStatImpact / 100.0f;
                _skillFilledImage.fillAmount += answerImpact._skillStatImpact / 100.0f;
                _fansAndMediaFilledImage.fillAmount += answerImpact._fansMediaStatImpact / 100.0f;
                _financesFilledImage.fillAmount += answerImpact._financeStatImpact / 100.0f;
                _personalLifeFilledImage.fillAmount += answerImpact._personalLifeStatImpact / 100.0f;
                break;

            case State.MATCH:
                MatchManager.Instance._formStat += answerImpact._formStatImpact;
                MatchManager.Instance._moralStat += answerImpact._moralStatImpact;
                MatchManager.Instance._aggroStat += answerImpact._aggroStatImpact;
                MatchManager.Instance._confidenceStat += answerImpact._confidenceStatImpact;
                MatchManager.Instance._disciplineStat += answerImpact._disciplineStatImpact;
                MatchManager.Instance._note += answerImpact._noteImpact;
                break;
        }
        
    }
}
