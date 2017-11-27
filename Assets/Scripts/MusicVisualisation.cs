﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// Root class for a single music visualisation
/// </summary>
public class MusicVisualisation : MonoBehaviour
{
    [Range(0, 1)]
    public float Strength = 1;

    private readonly List<MusicVisualisationComponent> _visualisations = new List<MusicVisualisationComponent>();

    private void Awake()
    {
        _visualisations.Clear();
        _visualisations.AddRange(GetComponentsInChildren<MusicVisualisationComponent>());
    }

    public void Think(float time, MusicState currentState)
    {
        foreach (var musicVisualisation in _visualisations)
        {
            musicVisualisation.Think(Strength, time, currentState);
        }
    }
}