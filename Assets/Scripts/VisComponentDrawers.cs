#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MadMaps.Common.GenericEditor;
using System;
using System.Reflection;
using MadMaps.Common;

/*public class VisComponentDrawer : GenericDrawer<VisComponent>
{
	protected override VisComponent DrawGUIInternal(VisComponent target, string label = "", Type targetType = null, FieldInfo fieldInfo = null,
		object context = null)
	{
		EditorGUILayout.LabelField("", GUILayout.Height(6), GUILayout.Width(MusicVisualisationGUI.ComponentSize));
		var last = GUILayoutUtility.GetLastRect();
		if(target.Listener == null)
		{
			EditorGUILayoutX.DerivedTypeSelectButton(typeof(IListener), (o) => 
				{
					fieldInfo.SetValue(context, o);
				}
			);
		}
		else 
		{
			EditorGUI.ProgressBar(last, target.Listener.Strength, "Strength");
		}		
		return target;
	}
}*/
#endif