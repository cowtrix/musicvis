using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MadMaps.Common.Serialization;
using System;

public class Listener : MonoBehaviour, ISerializationCallbackReceiver
{
	public IListener CurrentListener;
	[SerializeField][HideInInspector]
	private DerivedComponentJsonDataRow ListenerJSON;

	private void Update()
	{
		if(CurrentListener != null)
		{
			CurrentListener.Listen();
		}
	}

	public void OnBeforeSerialize()
    {
		if(CurrentListener != null)
		{
			ListenerJSON = new DerivedComponentJsonDataRow();
			ListenerJSON.AssemblyQualifiedName = CurrentListener.GetType().AssemblyQualifiedName;
			ListenerJSON.SerializedObjects = new List<UnityEngine.Object>();  
			ListenerJSON.JsonText = JSONSerializer.Serialize(CurrentListener.GetType(), CurrentListener, 
			false, ListenerJSON.SerializedObjects);
		}
		else
		{
			ListenerJSON = null;
		}		
    }

    public void OnAfterDeserialize()
    {        
		if(ListenerJSON == null)
		{			
			return;
		}
		CurrentListener = JSONSerializer.Deserialize(Type.GetType(ListenerJSON.AssemblyQualifiedName), 
			ListenerJSON.JsonText, ListenerJSON.SerializedObjects) as IListener;		
    }
}
