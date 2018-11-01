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
        var listener = target as Listener;
        if(Application.isPlaying)
        {
            EditorGUILayout.LabelField("Strength: " + listener.Strength);
        }
        GenericEditor.DrawGUI(target);
    }
}