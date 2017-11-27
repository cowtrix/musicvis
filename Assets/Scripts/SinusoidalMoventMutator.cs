using System.Configuration.Assemblies;
using UnityEngine;

public class SinusoidalMoventMutator : MusicVisualisationComponent
{
    public Vector3 Vector;
    public float Multiplier = .1f;
    public float Frequency = .1f;

    public ESampleMode Mode = ESampleMode.RMS;
    [Range(0, 1)]
    public float Wavelength = 0;

    private float _currentTime = 0;

    public override void Think(float strenghth, float time, MusicState currentState)
    {
        Value = GetValFromSampleMode(Mode, currentState, Wavelength);
        _currentTime += Time.deltaTime * Value * Frequency;
        transform.position = Mathf.Sin(_currentTime) * Vector * Multiplier;
    }
}