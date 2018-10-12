using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MadMaps.Common.Serialization;
using System;

/// <summary>
/// Root class for a single music visualisation
/// </summary>
public class MusicVisualisation : MonoBehaviour
{
    [Range(0, 1)]
    public float Strength = 1;

    List<Transition> _transitions = new List<Transition>();

    private void Awake()
    {
        _transitions.AddRange(GetComponentsInChildren<Transition>());
    }

    private void Update()
    {
        foreach(var transition in _transitions)
        {
            transition.Tick(Strength);
        }
    }
}