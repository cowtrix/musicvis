using UnityEngine;

public class RotationMutator : MusicVisualisationComponent
{
    public Vector3 Rotation = new Vector3(0, 1, 0);
    public float Multiplier = .1f;
    public ESampleMode Mode = ESampleMode.RMS;
    [Range(0, 1)]
    public float Wavelength = 0;
    
    public override void Think(float strenghth, float time, MusicState currentState)
    {
        Value = GetValFromSampleMode(Mode, currentState, Wavelength);
        var dt = Time.deltaTime * Value * Multiplier;
        transform.rotation *= Quaternion.Euler(dt * Rotation);
    }

    
}