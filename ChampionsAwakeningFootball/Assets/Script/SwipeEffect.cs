using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Playables;
using Unity.VisualScripting;

public class SwipeEffect : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Vector3 _initialPos;
    private float _distanceMoved;
    private bool _swipeLeft;
    private bool _dragStart = true;
    private bool canDrag = true;
    bool played = false;
    private Vector3 _lastPos;

    [SerializeField] private GameObject _topImg;
    [SerializeField] private GameObject _answer1, _answer2;
    
    void Update()
    {
        
        if(gameObject.transform == transform.parent.GetChild(0) && GetComponent<PlayableDirector>().state != PlayState.Playing && !played)
        {
            GetComponent<PlayableDirector>().Play();
            played = true;
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (!canDrag) return;
        float posX = transform.localPosition.x + eventData.delta.x;
        float slowFactor = 1 / (1 + Mathf.Log(Mathf.Abs(transform.localPosition.x) + 1) * 2f); // 0.5f pour ajuster la force
        transform.localPosition = new Vector2(transform.localPosition.x + eventData.delta.x * slowFactor * 6f, transform.localPosition.y);

        if (transform.localPosition.x - _initialPos.x > 0)
        {
            transform.localEulerAngles = new Vector3(0, 0, Mathf.LerpAngle(0,-20, (_initialPos.x+transform.localPosition.x)/(Screen.width/2)));
            _topImg.transform.localEulerAngles = new Vector3(0, 0, -.6f*(transform.localEulerAngles.z-360));
            if (_dragStart)
            {
                StartCoroutine(AnswerAppear(_answer1));
                _dragStart = false;
            }
            else
            {
                if(_lastPos.x - _initialPos.x < 0)
                {
                    InterruptAnimations();

                    StartCoroutine(AnswerDisappear(_lastPos.x));
                    StartCoroutine(AnswerAppear(_answer1));
                }
            }

        }
        else
        {
            transform.localEulerAngles = new Vector3(0, 0, Mathf.LerpAngle(0, +20, (_initialPos.x - transform.localPosition.x) / (Screen.width / 2)));
            _topImg.transform.localEulerAngles = new Vector3(0, 0, -.6f*(transform.localEulerAngles.z));
            if (_dragStart)
            {
                StartCoroutine(AnswerAppear(_answer2));
                _dragStart = false;
            }
            else
            {
                if (_lastPos.x - _initialPos.x > 0)
                {
                    InterruptAnimations();

                    StartCoroutine(AnswerDisappear(_lastPos.x));
                    StartCoroutine(AnswerAppear(_answer2));
                }
            }
        }
        _lastPos = transform.localPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!canDrag) return;
        _initialPos = new Vector3(0, transform.localPosition.y, transform.localPosition.z);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!canDrag) return;
        _distanceMoved = Mathf.Abs(transform.localPosition.x - _initialPos.x);
        if (_distanceMoved<0.25 * Screen.width)
        {
            StartCoroutine(MovedCardToInitialPos());
        }
        else
        {
            if (transform.localPosition.x > _initialPos.x)
            {
                _swipeLeft = false;
            }
            else
            {
                _swipeLeft = true;
            }
            StartCoroutine(MovedCard());
        }
    }

    public void SetDraggable(bool state)
    {
        canDrag = state;
    }

    private IEnumerator MovedCard()
    {
        float time = 0f;
        Color imgColor = GetComponent<Image>().color;
        while (GetComponent<Image>().color != new Color(imgColor.r, imgColor.g, imgColor.b, 0))
        {
            time += Time.deltaTime;
            if (_swipeLeft)
            {
                transform.localPosition = new Vector3(Mathf.SmoothStep(transform.localPosition.x, transform.localPosition.x - (Screen.width*0.03f), 4 * time), transform.localPosition.y, 0);
            }
            else
            {
                transform.localPosition = new Vector3(Mathf.SmoothStep(transform.localPosition.x, transform.localPosition.x + (Screen.width * 0.03f), 4 * time), transform.localPosition.y, 0);
            }
            GetComponent<Image>().color = new Color(imgColor.r, imgColor.g, imgColor.b, Mathf.SmoothStep(1,0,4*time));
            yield return null;
        }

        switch (_swipeLeft)
        {
            case false:
                ChoiceImpact(_answer1);
                break;
            case true:
                ChoiceImpact(_answer2);
                break;
        }
    }


    private IEnumerator MovedCardToInitialPos()
    {
        SetDraggable(false);
        float duration = 0.2f;
        float elapsedTime = 0f;
        float startX = transform.localPosition.x;
        float startZ = transform.localEulerAngles.z;
        float shortestAngle = Mathf.DeltaAngle(startZ, 0); // Shortest rotation angle

        StartCoroutine(AnswerDisappear());

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            float newX = Mathf.SmoothStep(startX, _initialPos.x, t);
            float newZ = Mathf.SmoothStep(startZ, startZ + shortestAngle, t);

            transform.localPosition = new Vector3(newX, transform.localPosition.y, 0);
            transform.localEulerAngles = new Vector3(0, 0, newZ);
            if(transform.localEulerAngles.z>180f)
                _topImg.transform.localEulerAngles = new Vector3(0, 0, -0.6f*(newZ-360));
            else _topImg.transform.localEulerAngles=new Vector3(0, 0,-0.6f*newZ);

            yield return null;
        }

        // Adjust the final position and rotation exactly as we want
        transform.localPosition = _initialPos;
        transform.localEulerAngles = Vector3.zero;
        _topImg.transform.localEulerAngles = Vector3.zero;
        SetDraggable(true);
        _dragStart = true;
        InterruptAnimations();
    }

    private IEnumerator AnswerAppear(GameObject answer)
    {
        float duration = 1f;
        float elapsedTime = 0f;
        Color answerColor = answer.GetComponent<TextMeshProUGUI>().color;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            answer.GetComponent<TextMeshProUGUI>().color = new Color(answerColor.r, answerColor.g, answerColor.b, t*3f);
            _topImg.transform.parent.localPosition = new Vector3(_topImg.transform.parent.localPosition.x, Mathf.SmoothStep(_topImg.transform.parent.localPosition.y, 470, t), _topImg.transform.parent.localPosition.z);
            yield return null;
        }
        answer.GetComponent<TextMeshProUGUI>().color = new Color(answerColor.r, answerColor.g, answerColor.b, 1);

    }

    private IEnumerator AnswerDisappear(float? lastPosX = null)
    {
        float duration = 0.2f;
        float elapsedTime = 0f;

        //Debug.Log("Swipe Left ? "+_swipeLeft);

        GameObject answer;
        if (lastPosX != null || lastPosX == 0.0f)
        {
            if (lastPosX < _initialPos.x) answer = _answer2;
            else answer = _answer1;
        }
        else
        {
            if (transform.localPosition.x < _initialPos.x) answer = _answer2;
            else answer = _answer1;
        }

        Color answerColor = answer.GetComponent<TextMeshProUGUI>().color;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            float newAlpha = Mathf.SmoothStep(answerColor.a, 0, t);
            answer.GetComponent<TextMeshProUGUI>().color = new Color(answerColor.r, answerColor.g, answerColor.b, newAlpha);
            _topImg.transform.parent.localPosition = new Vector3(_topImg.transform.parent.localPosition.x, Mathf.SmoothStep(_topImg.transform.parent.localPosition.y, 730, t), _topImg.transform.parent.localPosition.z);
            yield return null;
        }
        answer.GetComponent<TextMeshProUGUI>().color = new Color(answerColor.r, answerColor.g, answerColor.b, 0);
        _topImg.transform.parent.localPosition = new Vector3(_topImg.transform.parent.localPosition.x, 730, _topImg.transform.parent.localPosition.z);
    }

    void InterruptAnimations()
    {
        StopAllCoroutines();
        _answer1.GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 1, 0);
        _answer2.GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 1, 0);
    }

    private void ChoiceImpact(GameObject selectedAnswer)
    {
        AnswerImpact selectedAnswerImpact = selectedAnswer.GetComponent<AnswerImpact>();
        ImpactManager.Instance.ApplyImpact(selectedAnswerImpact);

        if (ImpactManager.Instance.state == ImpactManager.State.MATCH)
        {
            Debug.Log("Next card : " + selectedAnswerImpact._nextCard);

            // If there is a card linked to this choice and proc by the random, then we instantiate this card

            if (selectedAnswerImpact._nextCard != null)
            {
                Debug.Log("1");
                CardMatchManager.Instance.InitNewCard(selectedAnswerImpact._nextCard);
                Destroy(gameObject);
            }

            // Else we start the simulation back

            else
            {
                Debug.Log("2");
                foreach (Transform child in CardMatchManager.Instance._cardContainer)
                {
                    Destroy(child.gameObject);
                }
                
                MatchSimulation.Instance.isSimulating = true;
                MatchSimulation.Instance.ResimulateMatch();
                CardMatchManager.Instance.gameObject.SetActive(false);
            }
        }

        // And destroy the card we just swiped
        else
        {
            Destroy(gameObject);
        }

    }
}
