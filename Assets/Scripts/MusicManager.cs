using System;
using System.Collections.Generic;
using UnityEngine;

public class MusicState
{
    public float Peak;
    public float RMS;
    public float[] Wavelength = new float[512];
}

public class MusicManager : MonoBehaviour
{
    public float Silence = -40;
    public Lasp.FilterType FilterType;
    public MusicState MusicState = new MusicState();

    private List<MusicVisualisation> _visualisations;

    private void Awake()
    {
        _visualisations = new List<MusicVisualisation>(GetComponentsInChildren<MusicVisualisation>());
    }

    private void Update()
    {
        var peak = Lasp.AudioInput.GetPeakLevelDecibel(FilterType);
        var rms = Lasp.AudioInput.CalculateRMSDecibel(FilterType);
        Lasp.AudioInput.RetrieveWaveform(FilterType, MusicState.Wavelength);
        MusicState.Peak = Mathf.Clamp01(1 - peak / Silence);
        MusicState.RMS = Mathf.Clamp01(1 - rms / Silence);

        var t = Time.time;
        foreach (var musicVisualisation in _visualisations)
        {
            musicVisualisation.Think(t, MusicState);
        }
    }
}