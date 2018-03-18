using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Root class for a single music visualisation
/// </summary>
public class MusicVisualisation : MonoBehaviour
{
    [Range(0, 1)]
    public float Strength = 1;
    public float Time;
    public MusicState CurrentState;

    private readonly List<MusicVisualisationComponent> _visualisations = new List<MusicVisualisationComponent>();

    public ColorTemplateManager ColorTemplateManager;

    private void Awake()
    {
        _visualisations.Clear();
        _visualisations.AddRange(GetComponentsInChildren<MusicVisualisationComponent>());
    }

    public void Think(float time, MusicState currentState)
    {
        Time = time;
        CurrentState = currentState;

        for (var i = 0; i < _visualisations.Count; i++)
        {
            var musicVisualisation = _visualisations[i];
            if (musicVisualisation == null)
            {
                _visualisations.RemoveAt(i);
                i--;
                continue;
            }
            musicVisualisation.Think(this);
        }
    }
}