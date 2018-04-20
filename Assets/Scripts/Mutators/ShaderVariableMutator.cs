using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MadMaps.Common.GenericEditor;
using MadMaps.Common.Serialization;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(ShaderVariableMutator))]
public class ShaderVariableMutatorGUI : Editor
{
	public override void OnInspectorGUI()
	{
		GenericEditor.DrawGUI(target);
	}
}
#endif

public class ShaderVariableMutator : Mutator, ISerializationCallbackReceiver
{
	public interface IRenderable
	{
		void SetColor(string name, Color c);
		void SetFloat(string name, float f);
	}

	[Serializable]
	public class UnityRenderer : IRenderable
	{
		public Renderer Renderer;
		private static MaterialPropertyBlock _block;

		public void SetColor(string name, Color c)
		{
			if(_block == null)
			{
				_block = new MaterialPropertyBlock();
			}
			Renderer.GetPropertyBlock(_block);
			_block.SetColor(name, c);
			Renderer.SetPropertyBlock(_block); 				
		}

		public void SetFloat(string name, float f)
		{
			if(_block == null)
			{
				_block = new MaterialPropertyBlock();
			}
			Renderer.GetPropertyBlock(_block);
			_block.SetFloat(name, f);
			Renderer.SetPropertyBlock(_block); 	
		}
	}

	public interface IShaderVariable
	{
		void SetValue(object val);
	}

	public abstract class GenericShaderVariable<T> : IShaderVariable	
	{
		public void SetValue(object val)
		{
			SetValueInternal((T)val);
		}

		protected abstract void SetValueInternal(T val);
	}

	[Serializable]
	public class FloatVariable : GenericShaderVariable<float>
	{
		public string Name;
		public IRenderable Renderer;

		protected override void SetValueInternal(float val)
		{
			Renderer.SetFloat(Name, val);
		}
	}

	public IShaderVariable Variable;
	[SerializeField] DerivedComponentJsonDataRow VariableJSON;

	public override void Tick(float strength)
	{
		if(Variable != null)
		{
			Variable.SetValue(strength);
		}		
	}

	public void OnBeforeSerialize()
    {
        VariableJSON = new DerivedComponentJsonDataRow();
        if(Variable != null)
        {
			VariableJSON.SerializedObjects = new List<UnityEngine.Object>();
            VariableJSON.AssemblyQualifiedName = Variable.GetType().AssemblyQualifiedName;
            VariableJSON.JsonText = JSONSerializer.Serialize(Variable.GetType(), Variable, false, VariableJSON.SerializedObjects);
        }        
    }

    public void OnAfterDeserialize()
    {
        if(VariableJSON != null && !string.IsNullOrEmpty(VariableJSON.JsonText))
        {
            Variable = JSONSerializer.Deserialize(Type.GetType(VariableJSON.AssemblyQualifiedName), VariableJSON.JsonText, VariableJSON.SerializedObjects) as IShaderVariable;
        }            
    }
}
