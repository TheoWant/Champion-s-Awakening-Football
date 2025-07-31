using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class InvokeEvent : MonoBehaviour
{
    public enum InvokeMoment
    {
        Awake,
        Start,
        Update,
        Destroy,
        OnEnable
    }

    public UnityEvent unityEvent;

    [SerializeField] private InvokeMoment invokeMoment;

    private void Awake()
    {
        if (invokeMoment != InvokeMoment.Awake) { return; }
        unityEvent.Invoke();
    }

    void Start()
    {
        if (invokeMoment != InvokeMoment.Start) { return; }
        unityEvent.Invoke();
    }


    void Update()
    {
        if (invokeMoment != InvokeMoment.Update) { return; }
        unityEvent.Invoke();
    }

    void OnDestroy()
    {
        if (invokeMoment != InvokeMoment.Destroy) { return; }
        unityEvent.Invoke();
    }

    private void OnEnable()
    {
        if (invokeMoment != InvokeMoment.Start) { return; }
        unityEvent.Invoke();
    }
}
