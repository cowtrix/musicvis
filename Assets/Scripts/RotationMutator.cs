using UnityEngine;

public class RotationMutator : MusicVisualisationComponent
{
    public Vector3 Rotation = new Vector3(0, 1, 0);
    public float Multiplier = .1f;

    protected override void ThinkInternal(float strenghth, float time, MusicState currentState)
    {
        var dt = Time.deltaTime * Value.GetValue() * Multiplier;
        transform.rotation *= Quaternion.Euler(dt * Rotation);
    }
}