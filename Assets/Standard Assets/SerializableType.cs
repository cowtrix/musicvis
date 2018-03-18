using System;
using UnityEngine;

[Serializable]
public struct SerializableType
{
    //[HideInInspector]
    [SerializeField]
    private string _assemblyQualifiedName;

    public Type Type
    {
        get { return Type.GetType(_assemblyQualifiedName); }
        set { _assemblyQualifiedName = value.AssemblyQualifiedName; }
    }
}