using System;
using UnityEngine;

[Serializable]
public struct Triangle
{
    public Vector3 p1, p2, p3;

    public Triangle(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        this.p1 = p1;
        this.p2 = p2;
        this.p3 = p3;
    }

    public Vector3 normal()
    {
        return Vector3.Cross(p1 - p2, p1 - p3).normalized;
    }
}