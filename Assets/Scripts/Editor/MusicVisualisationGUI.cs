#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MadMaps.Common.GenericEditor;
using MadMaps.Common;
using System;
using System.Linq;

[CustomEditor(typeof(MusicVisualisation), true)]
public class MusicVisualisationGUI : Editor
{
    public override void OnInspectorGUI()
    {
        GenericEditor.DrawGUI(target);
        EditorUtility.SetDirty(target);
    }

    /*public static float ComponentSize = 120;
    private float _adjustedWidth;
    public static GUISkin Skin;
    public Vector2 DeskScroll;

    public List<VisComponent> Selected = new List<VisComponent>();
    
    private int MaxSelectedIndex
    {
        get 
        {
            if(Selected.Count == 0)
            {
                return -1;
            }
            return Selected.Max((o) => (target as MusicVisualisation).Components.LastIndexOf(o));
        }
    }

    public override void OnInspectorGUI()
    {
        //if(Skin == null)
        {
            Skin = Resources.Load<GUISkin>("VisSkin");
            //Skin =  GUI.skin;
        }
        GUI.skin = Skin;
        var musicVis = target as MusicVisualisation;
        float widthToUse = Screen.width - 32;
        int columnCount = Mathf.FloorToInt(widthToUse / ComponentSize) -1 ;
        _adjustedWidth = ComponentSize + (widthToUse - (ComponentSize * columnCount)) / (float)columnCount;

        _adjustedWidth = Mathf.Clamp(_adjustedWidth, 100, 200);

        int counter = 0;
        DeskScroll = EditorGUILayout.BeginScrollView(DeskScroll,  GUILayout.MaxWidth(Screen.width - 16), GUILayout.MaxHeight(Screen.height / 2));
        EditorGUILayout.BeginHorizontal("Desk", GUILayout.ExpandWidth(true));
        for (var i = 0; i < musicVis.Components.Count; ++i)
        {
            if(i < 0 || i >= musicVis.Components.Count)
            {
                continue;
            } 
            if(counter == columnCount)
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal("Desk", GUILayout.ExpandWidth(true));
                counter = 0;                
            }
            var temp = DrawComponent(musicVis.Components[i]);
            counter++;
            if(i >= musicVis.Components.Count)
            {
                continue;
            }     
            musicVis.Components[i] = temp;   
            if(musicVis.Components[i] == null)
            {
                musicVis.Components.RemoveAt(i);
                i--;
            }
        }

        if(counter == columnCount)
        {
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal("Desk", GUILayout.ExpandWidth(true));
            counter = 0;                
        }
        
        if(GUILayout.Button("+", "Component", GUILayout.Width(_adjustedWidth), GUILayout.Height(_adjustedWidth)))
        {
            var newComponent = Activator.CreateInstance(typeof(VisComponent)) as VisComponent;
            newComponent.Name = "New Component " + (musicVis.Components.Count +1);
            musicVis.Components.Add(newComponent);
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndScrollView(); 
    }

    public VisComponent DrawComponent(VisComponent component)
    {
        var musicVis = target as MusicVisualisation;
        var thisIndex = musicVis.Components.LastIndexOf(component);
        var maxSelectedIndex = MaxSelectedIndex;
        if(!Selected.Contains(component))
        {
            GUI.color = new Color(.8f, .8f, .8f, 1);
        }
        EditorGUILayout.BeginVertical("Component", GUILayout.Width(_adjustedWidth), GUILayout.Height(_adjustedWidth), GUILayout.MaxWidth(_adjustedWidth));
        GUI.color = Color.white;
        EditorGUILayout.BeginHorizontal();
        component.Name = EditorGUILayout.TextField(component.Name, GUILayout.ExpandWidth(true));
        if(thisIndex == MaxSelectedIndex)
        {
            if(GUILayout.Button(EditorGUIUtility.IconContent("TreeEditor.Trash"), EditorStyles.label, GUILayout.Width(20), GUILayout.Height(20)))
            {
                foreach (var selectedComponent in Selected)
                {
                    musicVis.Components.Remove(selectedComponent);
                }
                Selected.Clear();
            }
        }
        
        EditorGUILayout.EndHorizontal();
        component = GenericEditor.DrawGUI(component, "", typeof(VisComponent), typeof(VisComponent).GetField("Components"), musicVis) as VisComponent;
        GUILayout.Label("", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)); // Filler
        EditorGUILayout.EndVertical();
        var totalRect = GUILayoutUtility.GetLastRect();
        if(GUI.Button(totalRect, "", EditorStyles.label))
        {
            if(Event.current.shift)
            { 
                if(thisIndex > maxSelectedIndex)
                {
                    var temp = maxSelectedIndex;
                    maxSelectedIndex = thisIndex;
                    thisIndex = temp;
                }
                for(var i = maxSelectedIndex; i <= thisIndex; ++i)
                {
                    if(!Selected.Contains(musicVis.Components[i]))
                    {
                        Selected.Add(musicVis.Components[i]);
                    }
                }
            }
            else if (Event.current.control)
            {
                if(Selected.Contains(component))
                {                
                    Selected.Remove(component);
                }
                else
                {
                    Selected.Add(component);
                }
            }
            else if(Selected.Contains(component))
            {                
                Selected.Clear();
            }
            else
            {
                Selected.Clear();
                Selected.Add(component);
            }
        }
        return component;
    }*/

}
#endif