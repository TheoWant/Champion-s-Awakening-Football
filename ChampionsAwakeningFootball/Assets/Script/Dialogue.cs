using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textComponent;
    [SerializeField] string[] _lines;
    [SerializeField] float _textSpeed;

    [SerializeField] GameObject _nextTutoBox;

    private int _index;

    // Start is called before the first frame update
    void Start()
    {
        _textComponent.text = string.Empty;
        StartDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (_textComponent.text == _lines[_index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                _textComponent.text = _lines[_index];
            }
        }
    }

    void StartDialogue()
    {
        _index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        foreach (char c in _lines[_index].ToCharArray())
        {
            _textComponent.text += c;
            yield return new WaitForSeconds(_textSpeed);
        }
    }

    void NextLine()
    {
        if (_index < _lines.Length - 1)
        {
            _index++;
            _textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            if(_nextTutoBox != null)
            {
                _nextTutoBox.SetActive(true);
            }
            gameObject.SetActive(false);
        }
    }
}
