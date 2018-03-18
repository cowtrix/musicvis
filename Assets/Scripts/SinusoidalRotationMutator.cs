using UnityEngine;

public class SinusoidalRotationMutator : MusicVisualisationComponent
{
    [ContextMenuItem("Auto Base Rotation", "AutoBaseRotation")]
    public Vector3 BaseRotation;
    public Vector3 Rotation;
    public float Multiplier = .1f;
    public float Frequency = .1f;
    public bool Local = true;

    private float _currentTime = 0;

    void AutoBaseRotation()
    {
        BaseRotation = transform.localRotation.eulerAngles;
    }

    protected override void ThinkInternal(float time, MusicVisualisation musicVisualisation)
    {
        _currentTime += Time.deltaTime * Value.GetValue() * Frequency;
        var val = Mathf.Sin(_currentTime) * Multiplier;

        if (Local)
        {
            var additiveRotation = Quaternion.Euler(Rotation * (Mathf.Sin(_currentTime) * Multiplier));
            transform.localRotation = Quaternion.Euler(BaseRotation) * additiveRotation;
        }
        else
        {
            var additiveRotation = Quaternion.Euler(Rotation * (Mathf.Sin(_currentTime) * Multiplier));
            transform.rotation = Quaternion.Euler(BaseRotation) * additiveRotation;
        }
    }
}