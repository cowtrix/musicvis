using UnityEngine;

public class ScalerMusicVisComponent : MusicVisualisationComponent
{
    public Vector3 BaseScaler = Vector3.one;
    public Vector3 ScaleMultiplier = Vector3.one;
    public float Coefficient = 1;

    protected override void ThinkInternal(float time, MusicVisualisation musicVisualisation)
    {
        transform.localScale = BaseScaler + ScaleMultiplier * Value.GetValue() * Coefficient;
    }
}