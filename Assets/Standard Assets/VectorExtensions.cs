using UnityEngine;

public static class VectorExtensions
{
    public static Vector2 xz(this Vector3 vec)
    {
        return new Vector2(vec.x, vec.z);
    }

    public static Vector2 xy(this Vector3 vec)
    {
        return new Vector2(vec.x, vec.y);
    }

    public static Vector2 yz(this Vector3 vec)
    {
        return new Vector2(vec.y, vec.z);
    }

    public static Vector3 x0z(this Vector3 vec)
    {
        return new Vector3(vec.x, 0, vec.z);
    }

    public static Vector2 xy0(this Vector3 vec)
    {
        return new Vector3(vec.x, vec.y, 0);
    }

    public static Vector3 xy0(this Vector2 vec, float z = 0)
    {
        return new Vector3(vec.x, vec.y, z);
    }

    public static Vector3 x0yz(this Vector3 vec)
    {
        return new Vector3(0, vec.y, vec.z);
    }
}