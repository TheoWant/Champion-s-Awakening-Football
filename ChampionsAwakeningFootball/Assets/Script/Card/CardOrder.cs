using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardOrder : MonoBehaviour
{
    void Start()
    {
        int i = 0;
        foreach(Transform child in transform.parent)
        {
            if (child == transform)
            {
                child.gameObject.GetComponent<Canvas>().sortingOrder = 1000-i; 
                break;
            }
            i++;
        }
    }
}
