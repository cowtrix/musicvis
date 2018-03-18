using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(PercentFieldAttribute))]
public class PercentFieldAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        property.floatValue = EditorGUI.FloatField(position, string.Format("{0} (%)", property.displayName), property.floatValue);
        EditorGUI.LabelField(new Rect(position.width - 12, position.y, 16, position.height), "%");
    }
}