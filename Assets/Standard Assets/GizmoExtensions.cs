using UnityEngine;

public static class GizmoExtensions
{
    public static void Label(Vector3 position, string label)
    {
#if UNITY_EDITOR
        UnityEditor.Handles.Label(position, label);
#endif
    }

    public static void DrawWireCube(Vector3 origin, Vector3 extents, Quaternion rotation, Color color)
    {
        var verts = new[]
        {
            // Top square
            origin + rotation*new Vector3(extents.x, extents.y, extents.z),
            origin + rotation*new Vector3(-extents.x, extents.y, extents.z),
            origin + rotation*new Vector3(extents.x, extents.y, -extents.z),
            origin + rotation*new Vector3(-extents.x, extents.y, -extents.z),

            // Bottom square
            origin + rotation*new Vector3(extents.x, -extents.y, extents.z),
            origin + rotation*new Vector3(-extents.x, -extents.y, extents.z),
            origin + rotation*new Vector3(extents.x, -extents.y, -extents.z),
            origin + rotation*new Vector3(-extents.x, -extents.y, -extents.z)
        };

        Gizmos.color = color;

        // top square
        Gizmos.DrawLine(verts[0], verts[2]);
        Gizmos.DrawLine(verts[1], verts[3]);
        Gizmos.DrawLine(verts[1], verts[0]);
        Gizmos.DrawLine(verts[2], verts[3]);

        // bottom square
        Gizmos.DrawLine(verts[4], verts[6]);
        Gizmos.DrawLine(verts[5], verts[7]);
        Gizmos.DrawLine(verts[5], verts[4]);
        Gizmos.DrawLine(verts[6], verts[7]);

        // connections
        Gizmos.DrawLine(verts[0], verts[4]);
        Gizmos.DrawLine(verts[1], verts[5]);
        Gizmos.DrawLine(verts[2], verts[6]);
        Gizmos.DrawLine(verts[3], verts[7]);

        Gizmos.color = Color.white;
    }

    public static void DrawCapsule(Vector3 start, Vector3 end, float radius, Quaternion rotation, Color color)
    {
        // TODO - top cap slightly overdraws semi-circle (ie. greater than 180 degrees), investigate
        // Draw top cap
        DrawCircle(start, radius, rotation, 0, Mathf.PI, color);
        DrawCircle(start, radius, rotation * Quaternion.LookRotation(Vector3.right), 0, Mathf.PI, color);
        DrawCircle(start, radius, rotation * Quaternion.LookRotation(Vector3.up), color);
        // Draw bottom cap
        DrawCircle(end, radius, rotation, Mathf.PI + 0.01f, Mathf.PI * 2, color);
        DrawCircle(end, radius, rotation * Quaternion.LookRotation(Vector3.right), Mathf.PI + 0.01f, Mathf.PI * 2, color);
        DrawCircle(end, radius, rotation * Quaternion.LookRotation(Vector3.up), color);
        // Draw connectors
        
        Gizmos.color = color;
        Gizmos.DrawLine(start + rotation * (Vector3.right * radius), end + rotation * (Vector3.right * radius));
        Gizmos.DrawLine(start + rotation * (-Vector3.right * radius), end + rotation * (-Vector3.right * radius));
        Gizmos.DrawLine(start + rotation * (Vector3.forward * radius), end + rotation * (Vector3.forward * radius));
        Gizmos.DrawLine(start + rotation * (-Vector3.forward * radius), end + rotation * (-Vector3.forward * radius));
        Gizmos.color = Color.white;
    }

    public static void DrawSphere(Vector3 origin, Quaternion rotation, float radius, Color color)
    {
#if !UNITY_EDITOR
        return;
#else
        // Draw top cap
        DrawCircle(origin, radius, rotation, color);
        DrawCircle(origin, radius, rotation * Quaternion.LookRotation(Vector3.right), color);
        DrawCircle(origin, radius, rotation * Quaternion.LookRotation(Vector3.up), color);
#endif
    }

    public static void DrawCircle(Vector3 origin, float radius, Quaternion rotation, Color color)
    {
#if !UNITY_EDITOR
        return;
#else
        DrawCircle(origin, radius, rotation, float.MinValue, float.MaxValue, color);
#endif
    }

    public static void DrawCircle(Vector3 origin, float radius, Quaternion rotation, float startAngle, float endAngle, Color color)
    {
#if !UNITY_EDITOR
        return;
#else
        Gizmos.color = color;
        float resolution = 24;
        Vector3 lastPoint = Vector3.zero;
        for (var i = 0; i <= resolution; ++i)
        {
            float angle = (i / resolution) * Mathf.PI * 2;

            float x = Mathf.Cos(angle);
            float y = Mathf.Sin(angle);
            var thisPoint = new Vector3(x, y, 0);
            thisPoint = origin + rotation * (thisPoint * radius);
            if (i > 0)
            {
                if (Mathfx.BetweenInclusive(angle, startAngle, endAngle))
                {
                    Gizmos.DrawLine(lastPoint, thisPoint);
                }
            }
            lastPoint = thisPoint;
        }
        Gizmos.color = Color.white;
#endif
    }
}