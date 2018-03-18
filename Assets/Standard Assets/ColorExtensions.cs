using UnityEngine;

public static class ColorExtensions
{
    public static Color WithAlpha(this Color col, float alpha)
    {
        return new Color(col.r, col.g, col.b, alpha);
    }
}