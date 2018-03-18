using UnityEngine;

public static class CurveExtensions
{
    public static float Integrate(this AnimationCurve curve, int steps = 100)
    {
        float minTime = curve.MinTime();
        float maxTime = curve.MaxTime();
        float delta = maxTime - minTime;
        float tDelta = (1 / (float)steps) * delta;
        float sum = 0f;
        for (var t = minTime; t < maxTime; t += tDelta)
        {
            sum += curve.Evaluate(t);
        }
        return sum;
    }

    public static float MinTime(this AnimationCurve curve)
    {
        if (curve.keys.Length == 0)
        {
            return float.NaN;
        }
        return curve.keys[0].time;
    }

    public static float MaxTime(this AnimationCurve curve)
    {
        if (curve.keys.Length == 0)
        {
            return float.NaN;
        }
        return curve.keys[curve.keys.Length - 1].time;
    }
}
