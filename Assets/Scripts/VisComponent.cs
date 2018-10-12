using UnityEngine;
using System.Collections.Generic;
using MadMaps.Common.Serialization;
using System;
/* 
public class VisComponent
{
    public String Name = "New Component";
    [Range(0, 1)]
    public float Weight = 1;
    
    public IListener Listener;
    public List<IMutatorEffector> MutatorEffectors = new List<IMutatorEffector>();
    
    protected virtual void Awake()
    {
    }

    public void Think(MusicVisualisation musicVisualisation)
    {
        Listener.Listen(musicVisualisation.CurrentState);
        var strength = Listener.Strength * Weight;
        foreach (var mutator in MutatorEffectors)
        {
            mutator.Tick(strength);
        }
    }
    
    public override string ToString()
    {
        if(Listener == null)
        {
            return Name + " (NO LISTENER)";
        }
        return string.Format("{2} : {0} : {1}", Listener.Strength, Listener.ToString(), Name);
    }
}*/