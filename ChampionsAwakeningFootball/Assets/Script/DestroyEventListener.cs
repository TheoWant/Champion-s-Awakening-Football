using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DestroyEventListener : MonoBehaviour
{
    public GameObject gameObjectToActivate;
    private void OnDestroy()
    {
        if(gameObjectToActivate != null)
        {
            gameObjectToActivate.SetActive(true);
        }
    }
}
