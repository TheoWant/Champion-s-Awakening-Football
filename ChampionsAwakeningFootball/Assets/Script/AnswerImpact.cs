using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnswerImpact : MonoBehaviour
{
    [Header("Main game stat")]
    [SerializeField] public float _teamStatImpact;
    [SerializeField] public float _skillStatImpact;
    [SerializeField] public float _fansMediaStatImpact;
    [SerializeField] public float _financeStatImpact;
    [SerializeField] public float _personalLifeStatImpact;

    [Header("Match stat")]
    [SerializeField] public float _formStatImpact;
    [SerializeField] public float _moralStatImpact;
    [SerializeField] public float _aggroStatImpact;
    [SerializeField] public float _confidenceStatImpact;
    [SerializeField] public float _disciplineStatImpact;
    [SerializeField] public float _noteImpact;
    [SerializeField] public CardMatch _nextCard;
}
