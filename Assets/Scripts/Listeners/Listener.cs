using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MadMaps.Common.Serialization;
using System;
using MidiJack;

public class Listener : MonoBehaviour, ISerializationCallbackReceiver
{
	public const int MIN_BUFFER = 1;
	public const int MAX_BUFFER = 2000;
	public string Name;

	[SerializeField]
	private IListener CurrentListener;
	public SmartValue Value;
	public bool Hidden;
	

	[SerializeField][HideInInspector]
	private DerivedComponentJsonDataRow ListenerJSON;

	[Header("MIDI")]
	public bool UseMidi;
	public MidiChannel MidiChannel;
	public int MidiIndex;
	public FloatEvent SimplePipe;

    public string GetName()
    {
        if(string.IsNullOrEmpty(Name))
		{
			return name;
		}
		return Name;
    }

    

    public float Strength 
	{ 
		get
		{
			if(CurrentListener != null)
			{
				return Value.GetValue();
			}
			return 1;
		}
	}

	void OnDrawGizmos()
	{
		Value.BufferSize = Mathf.Clamp(Value.BufferSize, MIN_BUFFER, MAX_BUFFER);
		Value.Smooth = Mathf.Clamp01(Value.Smooth);		
	}

	void TryRegister()
	{
		if(GUIManager.HasInstance())
		{
			GUIManager.Instance.RegisterListener(this);
		}
		if(MusicManager.HasInstance())
		{
			MusicManager.Instance.RegisterListener(this);
		}
	}

	void Update()
	{
		TryRegister();
		Value.Tick(Time.deltaTime);
		if(SimplePipe != null)
		{
			SimplePipe.Invoke(Value.GetValue());
		}
	}

    public void Tick()
	{	
			
		if(CurrentListener != null)
		{
			CurrentListener.Listen(Value);
		}
		#if UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN    // MIDI don't work with Linux :(
		if(UseMidi)
		{
			Value.Multiplier = MidiMaster.GetKnob(MidiChannel, MidiIndex);
		}
		#endif
	}

	void Awake()
	{
		TryRegister();
	}

	void OnEnable()
	{
		TryRegister();
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
