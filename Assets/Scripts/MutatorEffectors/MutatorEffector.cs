using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

[Serializable]
public class FloatEvent : UnityEvent<float>{}

public abstract class MutatorEffector : MonoBehaviour 
{
    public Listener Listener;
    public float Value {get; protected set;}
    public FloatEvent Event;

    private void Reset()
    {
        if(Listener == null)
        {
            Listener = GetComponent<Listener>();
        }
    }

    private void Update()
    {
        if(Listener == null)
        {
            Listener = GetComponent<Listener>();
            return;
        }
        Tick(Listener.Strength);
        if(Event != null)
        {
            Event.Invoke(Value);
        }
    }

    protected abstract void Tick(float value);
}
