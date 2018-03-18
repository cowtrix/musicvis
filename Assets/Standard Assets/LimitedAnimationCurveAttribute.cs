using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitedAnimationCurveAttribute : PropertyAttribute
{
    public Rect Limit;

    public LimitedAnimationCurveAttribute(float x, float y, float width, float height)
    {
        Limit = new Rect(x, y, width, height);
    }
}