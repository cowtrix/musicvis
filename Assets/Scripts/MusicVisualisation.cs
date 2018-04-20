using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MadMaps.Common.Serialization;
using System;
using MidiJack;

/// <summary>
/// Root class for a single music visualisation
/// </summary>
public class MusicVisualisation : MonoBehaviour, ISerializationCallbackReceiver
{
    public MidiChannel Channel;
    public int index = 0;

    [Range(0, 1)]
    public float Strength = 1;
    public float Time;
    [HideInInspector]
    public MusicState CurrentState = new MusicState();    
    public ColorTemplateManager ColorTemplateManager;

    public List<VisComponent> Components = new List<VisComponent>();
    [HideInInspector]
    public List<DerivedComponentJsonDataRow> ComponentsJSON = new List<DerivedComponentJsonDataRow>();

    public void Think(float time, MusicState currentState)
    {
        Strength = MidiMaster.GetKnob(Channel, index, 1);

        Time = time;
        CurrentState = currentState;

        for (var i = 0; i < Components.Count; i++)
        {
            var musicVisualisation = Components[i];
            if (musicVisualisation == null)
            {
                Components.RemoveAt(i);
                i--;
                continue;
            }
            musicVisualisation.Think(this);
        }
    }

    public void OnBeforeSerialize()
    {
        ComponentsJSON.Clear();
        foreach(var component in Components)
        {
            if(component == null)
            {
                ComponentsJSON.Add(null);
                continue;
            }
            var newRow = new DerivedComponentJsonDataRow();
            newRow.AssemblyQualifiedName = component.GetType().AssemblyQualifiedName;
            newRow.SerializedObjects = new List<UnityEngine.Object>();  
            newRow.JsonText = JSONSerializer.Serialize(component.GetType(), component, false, newRow.SerializedObjects);
            ComponentsJSON.Add(newRow);
        }
    }

    public void OnAfterDeserialize()
    {
        Components.Clear();
        foreach(var row in ComponentsJSON)
        {
            if(row == null)
            {
                Components.Add(null);
                continue;
            }
            var newObj = JSONSerializer.Deserialize(Type.GetType(row.AssemblyQualifiedName), row.JsonText, row.SerializedObjects) as VisComponent;
            Components.Add(newObj);
        }         
    }

}