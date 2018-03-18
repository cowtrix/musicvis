using ParadoxNotion.Design;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class ExpNumberDrawer : ObjectDrawer<ExpNumber>
{
    public override ExpNumber OnGUI(GUIContent content, ExpNumber instance)
    {
        EditorGUILayout.BeginHorizontal();
        instance.Value = EditorGUILayout.DoubleField(content, instance.Value);
        //EditorGUILayout.LabelField("x10^", EditorStyles.label, GUILayout.Width(60));
        instance.Exponent = (sbyte) EditorGUILayout.IntField(instance.Exponent, GUILayout.Width(100));
        EditorGUILayout.EndHorizontal();
        return instance;
    }
}