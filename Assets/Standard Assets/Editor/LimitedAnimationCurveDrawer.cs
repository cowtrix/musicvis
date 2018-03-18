using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(LimitedAnimationCurveAttribute), true)]
public class LimitedAnimationCurveDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var attr = attribute as LimitedAnimationCurveAttribute;
        var limit = attr.Limit;
        EditorGUI.CurveField(position, property.animationCurveValue, Color.white, limit);
    }
}