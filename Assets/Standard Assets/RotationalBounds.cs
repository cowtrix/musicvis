using System;
using UnityEngine;

[Serializable]
public struct RotationalBounds
{
    public Quaternion rotation;
    public Vector3 center;
    public Vector3 extents;

    public RotationalBounds(Vector3 center, Vector3 size, Quaternion rotation)
    {
        this.rotation = rotation;
        this.center = center;
        this.extents = size / 2;
    }

    public bool Overlaps(RotationalBounds b)
    {
        return Geometry3D.RectangularPrismOverlaps(center, extents, rotation, b.center, b.extents, b.rotation);
    }
    
    public Bounds GetAxisBounds()
    {
        var p1 = center + rotation * new Vector3(extents.x, 0, 0);
        var p2 = center + rotation * new Vector3(-extents.x, 0, 0);
        var p3 = center + rotation * new Vector3(0, extents.y, 0);
        var p4 = center + rotation * new Vector3(0, -extents.y, 0);
        var p5 = center + rotation * new Vector3(0, 0, extents.z);
        var p6 = center + rotation * new Vector3(0, 0, -extents.z);
        var b = new Bounds(p1, Vector3.zero);
        b.Encapsulate(p2);
        b.Encapsulate(p3);
        b.Encapsulate(p4);
        b.Encapsulate(p5);
        b.Encapsulate(p6);
        return b;
    }

    public override string ToString()
    {
        return string.Format("Position: {0} | Size: {1} | Rotation: {2}", center, extents * 2, rotation.eulerAngles);
    }
}