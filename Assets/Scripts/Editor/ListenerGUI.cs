using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MadMaps.Common.GenericEditor;
using MadMaps.Common;
using System;
using System.Linq;

[CustomEditor(typeof(Listener), true)]
public class ListenerGUI : Editor
{
    public override void OnInspectorGUI()
    {
        if(Application.isPlaying)
        {
            var listener = target as Listener;
            if(listener.CurrentListener != null)
            {
                EditorGUILayout.LabelField("Strength: " + listener.CurrentListener.Strength);
            }
        }
        GenericEditor.DrawGUI(target);
    }
}