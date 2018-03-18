using System.Configuration.Assemblies;
using UnityEngine;

public class SinusoidalMoventMutator : MusicVisualisationComponent
{
    public enum EAxis
    {
        X, Y, Z
    }

    public float Offset;
    public float Multiplier = .1f;
    public float Frequency = .1f;
    public bool Local = true;
    public EAxis Axis;
    
    private float _currentTime = 0;

    protected override void ThinkInternal(float time, MusicVisualisation musicVisualisation)
    {
        _currentTime += Time.deltaTime * Value.GetValue() * Frequency;
        var val = Offset + Mathf.Sin(_currentTime) * Multiplier;

        if (Local)
        {
            switch (Axis)
            {
                case EAxis.X:
                    transform.localPosition = new Vector3(val, transform.localPosition.y,
                        transform.localPosition.z);
                    break;
                case EAxis.Y:
                    transform.localPosition = new Vector3(transform.localPosition.x, val,
                        transform.localPosition.z);
                    break;
                case EAxis.Z:
                    transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y,
                        val);
                    break;
            }
        }
        else
        {
            //transform.position += transform.rotation * (Mathf.Sin(_currentTime) * Vector * Multiplier);
        }
    }
}