﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion;
using ParadoxNotion.Design;
using ParadoxNotion.Services;
using NodeCanvas.Framework;
using UnityEngine;

namespace FlowCanvas.Nodes{


	[Name("Identity Value")]
	[Category("Functions/Utility")]
	[Description("Use this for organization. It returns exactly what is provided in the input.")]
	public class Identity<T> : PureFunctionNode<T, T>{
		public override string name{get{return null;}}
		public override T Invoke(T value){
			return value;
		}
	}

/*
	[Category("Functions/Utility")]
	public class SelectValue<T> : PureFunctionNode<T, int, T, T, T, T>{
		public override T Invoke(int index, T a, T b, T c, T d){
			var l = new T[]{a, b, c, d};
			return l[index];
		}
	}
*/
	[Obsolete]
	[Category("Functions/Utility")]
	[Description("Return a value from the list by index")]
	public class PickValue<T> : PureFunctionNode<T, int, IList<T>>{
		public override T Invoke(int index, IList<T> values){
			try {return values[index];}
			catch {return default(T);}
		}
	}

	[Category("Functions/Utility")]
	[Description("Returns either one of the two inputs, based on the boolean condition")]
	public class SwitchValue<T> : PureFunctionNode<T, bool, T, T>{
		public override T Invoke(bool condition, T isTrue, T isFalse){
			return condition? isTrue : isFalse;
		}
	}


	[Category("Functions/Utility")]
	[Description("Get a component attached on an object")]
	public class GetComponent<T> : PureFunctionNode<T, GameObject> where T:Component{
		private T _component;
		public override T Invoke(GameObject gameObject){
			if (gameObject == null) return null;
			if (_component == null || _component.gameObject != gameObject){
				_component = gameObject.GetComponent<T>();			
			}
			return _component;
		}
	}

#if UNITY_5_5_OR_NEWER
	[Category("Functions/Utility")]
	[Description("Instantiate an object")]
	public class Instantiate<T> : CallableFunctionNode<T, T, Vector3, Quaternion, Transform> where T:UnityEngine.Object{
		public override T Invoke(T original, Vector3 position, Quaternion rotation, Transform parent){
			if (original == null) return null;
			return UnityEngine.Object.Instantiate<T>(original, position, rotation, parent);
		}
	}
#endif

	[Category("Functions/Utility")]
	[Description("Get all child transforms of specified parent")]
	public class GetChildTransforms : PureFunctionNode<Transform[], Transform>{
		public override Transform[] Invoke(Transform parent){
			return parent.Cast<Transform>().ToArray();
		}
	}


	[Category("Functions/Utility")]
	[Description("Wait for a certain amount of time before continueing")]
	public class Wait : LatentActionNode<float>{

		public float timeLeft{ get; private set; }

		public override IEnumerator Invoke(float time = 1f){
			timeLeft = time;
			while (timeLeft > 0){
				timeLeft -= Time.deltaTime;
				timeLeft = Mathf.Max(timeLeft, 0);
				yield return null;
			}
		}
	}


	[Category("Functions/Utility")]
	[Description("Caches the value only when the node is called.")]
	public class Cache<T> : CallableFunctionNode<T, T>{
		public override T Invoke(T value){
			return value;
		}
	}


	[Category("Functions/Utility")]
	[Description("Log input value on the console")]
	public class LogValue : CallableActionNode<object>{
		public override void Invoke(object obj){
			Debug.Log(obj);
		}
	}

	[Category("Functions/Utility")]
	[Description("Log text in the console")]
	public class LogText : CallableActionNode<string>{
		public override void Invoke(string text){
			Debug.Log(text);
		}
	}

	[Category("Functions/Utility")]
	[Description("Send a Local Event to specified graph")]
	public class SendEvent : CallableActionNode<GraphOwner, string>{
		public override void Invoke(GraphOwner target, string eventName){
			target.SendEvent(new EventData(eventName));
		}
	}

	[Category("Functions/Utility")]
	[Description("Send a Local Event with 1 argument to specified graph")]
	public class SendEvent<T> : CallableActionNode<GraphOwner, string, T>{
		public override void Invoke(GraphOwner target, string eventName, T eventValue){
			target.SendEvent(new EventData<T>(eventName, eventValue));
		}
	}

	[Category("Functions/Utility")]
	[Description("Send a Global Event to all graphs")]
	public class SendGlobalEvent : CallableActionNode<string>{
		public override void Invoke(string eventName){
			Graph.SendGlobalEvent(new EventData(eventName));
		}
	}

	[Category("Functions/Utility")]
	[Description("Send a Global Event with 1 argument to all graphs")]
	public class SendGlobalEvent<T> : CallableActionNode<string, T>{
		public override void Invoke(string eventName, T eventValue){
			Graph.SendGlobalEvent(new EventData<T>(eventName, eventValue));
		}
	}


	[Name("Per Second (Float)")]
	[Category("Functions/Utility")]
	[Description("Mutliply input value by Time.deltaTime and optional multiplier")]
	public class DeltaTimed : PureFunctionNode<float, float>{
		public override float Invoke(float value){
			return value * Time.deltaTime;
		}
	}

	[Name("Per Second (Vector3)")]
	[Category("Functions/Utility")]
	[Description("Mutliply input value by Time.deltaTime and optional multiplier")]
	public class DeltaTimedVector3 : PureFunctionNode<Vector3, Vector3>{
		public override Vector3 Invoke(Vector3 value){
			return value * Time.deltaTime;
		}
	}

}