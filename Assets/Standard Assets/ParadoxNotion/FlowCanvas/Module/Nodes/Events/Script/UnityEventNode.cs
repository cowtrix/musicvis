using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using UnityEngine;
using System;
using System.Reflection;
using UnityEngine.Events;

namespace FlowCanvas.Nodes{

	[Name("Unity Event")]
	[Description("Subscribes to a UnityEvent and is called when the event is raised")]
	[Category("Events/Script")]
	abstract public class UnityEventNodeBase : EventNode<Transform>{

		[SerializeField]
		protected string eventName;
		[SerializeField]
		protected Type targetType;

		protected Component targetComponent;
		protected FieldInfo eventField{
			get {return targetType != null? targetType.RTGetField(eventName) : null;}
		}


		public void SetEvent(FieldInfo field, object instance = null){
			targetType = field.RTReflectedType();
			eventName = field.Name;
			if (instance is Component){
				base.target.value = (instance as Component).transform;
			}
			GatherPorts();
		}

		public override void OnGraphStarted(){
			
			ResolveSelf();
			
			if (string.IsNullOrEmpty(eventName)){
				Debug.LogError("No Event Selected for CodeEvent, or target is NULL");
				return;
			}

			targetComponent = target.value.GetComponent(targetType);
			if (targetComponent == null){
				Debug.LogError("Target is null");
				return;
			}

			if (eventField == null){
				Debug.LogError("Event was not found");
				return;
			}
		}
	}

	///----------------------------------------------------------------------------------------------

	public class UnityEventNode : UnityEventNodeBase {

		private FlowOutput o;
		private UnityEvent unityEvent;

		public override void OnGraphStarted(){
			base.OnGraphStarted();
			unityEvent = (UnityEvent)eventField.GetValue(targetComponent);
			unityEvent.AddListener(Raised);
		}

		public override void OnGraphStoped(){
			if (!string.IsNullOrEmpty(eventName) && unityEvent != null){
				unityEvent.RemoveListener(Raised);
			}
		}

		void Raised(){
			o.Call(Flow.New);
		}

		protected override void RegisterPorts(){
			if (!string.IsNullOrEmpty(eventName)){
				o = AddFlowOutput(eventName.SplitCamelCase(), eventName);
			}
		}



		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR
			
		protected override void OnNodeInspectorGUI(){

			base.OnNodeInspectorGUI();

			if (eventName == null && !Application.isPlaying && GUILayout.Button("Select Event")){
				var o = target.value == null? graphAgent.gameObject : target.value.gameObject;
				EditorUtils.ShowGameObjectFieldSelectionMenu(o, typeof(UnityEvent), (e)=>{ SetEvent(e); });
			}

			if (eventName != null){
				GUILayout.BeginVertical("box");
				UnityEditor.EditorGUILayout.LabelField("Selected Type", targetType.FriendlyName());
				UnityEditor.EditorGUILayout.LabelField("Selected Event", eventName);
				GUILayout.EndVertical();
			}				
		}

		#endif
	}


	///----------------------------------------------------------------------------------------------

	public class UnityEventNode<T> : UnityEventNodeBase {

		private FlowOutput o;
		private T eventValue;
		private UnityEvent<T> unityEvent;

		public override void OnGraphStarted(){
			base.OnGraphStarted();
			unityEvent = (UnityEvent<T>)eventField.GetValue(targetComponent);
			unityEvent.AddListener(Raised);
		}

		void Raised(T value){
			eventValue = value;
			o.Call(Flow.New);
		}

		public override void OnGraphStoped(){
			if (!string.IsNullOrEmpty(eventName) && unityEvent != null){
				unityEvent.RemoveListener(Raised);
			}
		}

		protected override void RegisterPorts(){
			if (!string.IsNullOrEmpty(eventName)){
				o = AddFlowOutput(eventName.SplitCamelCase(), eventName);
				AddValueOutput<T>("Value", ()=>{ return eventValue; });
			}
		}


		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR
			
		protected override void OnNodeInspectorGUI(){

			base.OnNodeInspectorGUI();

			if (eventName == null && !Application.isPlaying && GUILayout.Button("Select Event")){
				var o = target.value == null? graphAgent.gameObject : target.value.gameObject;
				EditorUtils.ShowGameObjectFieldSelectionMenu(o, typeof(UnityEvent<T>), (e)=>{ SetEvent(e); });
			}

			if (eventName != null){
				GUILayout.BeginVertical("box");
				UnityEditor.EditorGUILayout.LabelField("Selected Type", targetType.FriendlyName());
				UnityEditor.EditorGUILayout.LabelField("Selected Event", eventName);
				GUILayout.EndVertical();
			}				
		}

		#endif
	}

}