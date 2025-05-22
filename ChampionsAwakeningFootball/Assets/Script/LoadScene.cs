using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    [SerializeField] public string sceneName;

    [SerializeField] bool loadSceneOnEnable;
    [SerializeField] bool tutoDone;

    [SerializeField] FadeScreen fadeOut;

    public void LoadSceneOnClick()
    {
        SceneManager.LoadScene(sceneName);
    }

    private void OnEnable()
    {
        if (tutoDone)
        {
            PlayerManager.Instance._isTutoDone = true;
            PlayerManager.Instance.UpdatePlayerInDatabase();
        }
        if (loadSceneOnEnable)
        {
            if (fadeOut != null)
            {
                StartCoroutine(LoadSceneAfterFade());
            }
            else
            {
                SceneManager.LoadScene(sceneName);
            }
        }
    }

    public IEnumerator LoadSceneAfterFade()
    {
        yield return StartCoroutine(fadeOut.FadeRoutine(0,1));

        SceneManager.LoadScene(sceneName);
    }
}
