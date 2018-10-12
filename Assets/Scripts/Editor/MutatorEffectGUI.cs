using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(MutatorEffector), true)]
public class MutatorEffectGUI : Editor 
{
	public override void OnInspectorGUI()
	{
		if(Application.isPlaying)
        {
            var mutatorEffector = target as MutatorEffector;
            EditorGUILayout.LabelField("Value: " + mutatorEffector.Value);
        }
		DrawDefaultInspector();		
	}
}
