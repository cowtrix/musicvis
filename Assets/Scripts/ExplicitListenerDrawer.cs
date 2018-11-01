#if UNITY_EDITOR
using UnityEditor;
using MadMaps.Common.GenericEditor;
using System.Reflection;
using System;
using UnityEngine;
using MidiJack;

public class ExplicitListenerDrawer : GenericDrawer<ExplicitListener>
{
	protected override ExplicitListener DrawGUIInternal(ExplicitListener target, string label = "", Type targetType = null, FieldInfo fieldInfo = null,
            object context = null)
	{
		GUILayout.Label(label);
		EditorGUI.indentLevel++;
		EditorGUILayout.BeginHorizontal();
		target.Value = EditorGUILayout.Slider(target.Value, target.Min, target.Max);
		if (GUILayout.Button(GenericEditor.DeleteContent, EditorStyles.label, GUILayout.Width(20)))
		{
			EditorGUI.indentLevel--;
			return null;
		}
		EditorGUILayout.EndHorizontal();

		var minMax = new Vector2(target.Min, target.Max);
		minMax = EditorGUILayout.Vector2Field(GUIContent.none, minMax);
		target.Min = minMax.x;
		target.Max = minMax.y;
		EditorGUI.indentLevel--;
		return target;
	}
}
#endif