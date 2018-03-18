using System;
using UnityEngine;

public class CircularMovementMutator : MusicVisualisationComponent
{
    public Vector3 Axis = new Vector3(0, 1, 0);
    public float Radius = 100;
    public float Multiplier = .1f;
    public float Offset = 0;
    public float CycleLength = 10;
    
    private float _angle;

    protected override void ThinkInternal(float time, MusicVisualisation musicVisualisation)
    {
        _angle = (Value.GetValue() + Offset);
        transform.position = transform.parent.position + new Vector3(Mathf.Cos(_angle) * Radius, Mathf.Sin(_angle) * Radius);
    }

    private void OnDrawGizmos()
    {
        GizmoExtensions.DrawCircle(transform.parent.position, Radius, Quaternion.AngleAxis(0, Axis) * Quaternion.Euler(new Vector3(0, 90, 0)), Color.white);
    }
}