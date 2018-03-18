using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using UnityEngine;
using System;
using System.Reflection;
using UnityEngine.Events;

namespace FlowCanvas.Nodes{

	[Name("Static Unity Event")]
	[Description("Subscribes to a static UnityEvent and is called when the event is raised")]
	[Category("Events/Script")]
	public class StaticUnityEventNode : EventNode {

		[SerializeField]
		private string eventName;
		[SerializeField]
		private Type targetType;

		private FlowOutput o;
		private UnityEvent unityEvent;


		public void SetEvent(FieldInfo f){
			targetType = f.RTReflectedType();
			eventName = f.Name;
			GatherPorts();			
		}

		public override void OnGraphStarted(){

			if (string.IsNullOrEmpty(eventName)){
				Debug.LogError("No Event Selected for 'Static Code Event'");
				return;
			}

			var eventField = targetType.RTGetField(eventName);
			if (eventField == null){
				Debug.LogError("Event was not found");
				return;
			}

			base.OnGraphStarted();

			unityEvent = (UnityEvent)eventField.GetValue(null);
			unityEvent.AddListener(Raised);
		}

		void Raised(){
			o.Call(Flow.New);
		}

		public override void OnGraphStoped(){

			if (string.IsNullOrEmpty(eventName)){
				return;
			}

			if (unityEvent != null){
				unityEvent.RemoveListener(Raised);
			}
		}

		protected override void RegisterPorts(){
			if (!string.IsNullOrEmpty(eventName)){
				o = AddFlowOutput(eventName);
			}
		}


		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR
			
		protected override void OnNodeInspectorGUI(){

			base.OnNodeInspectorGUI();

			if (eventName == null && !Application.isPlaying && GUILayout.Button("Select Event")){
				var menu = new UnityEditor.GenericMenu();
				foreach (var t in UserTypePrefs.GetPreferedTypesList(typeof(object))){
					menu = EditorUtils.GetStaticFieldSelectionMenu(t, typeof(UnityEvent), SetEvent, menu);
				}
				menu.ShowAsContext();
				Event.current.Use();
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



	[Name("Static Unity Event")]
	[Description("Subscribes to a static UnityEvent and is called when the event is raised")]
	[Category("Events/Script")]
	public class StaticUnityEventNode<T> : EventNode {

		[SerializeField]
		private string eventName;
		[SerializeField]
		private Type targetType;

		private FlowOutput o;
		private T eventValue;
		private UnityEvent<T> unityEvent;

		public void SetEvent(FieldInfo f){
			targetType = f.RTReflectedType();
			eventName = f.Name;
			GatherPorts();			
		}

		public override void OnGraphStarted(){
			
			if (string.IsNullOrEmpty(eventName)){
				Debug.LogError("No Event Selected for 'Static Code Event'");
				return;
			}

			var eventField = targetType.RTGetField(eventName);
			if (eventField == null){
				Debug.LogError("Event was not found");
				return;
			}

			base.OnGraphStarted();

			unityEvent = (UnityEvent<T>)eventField.GetValue(null);
			unityEvent.AddListener(Raised);
		}

		void Raised(T value){
			eventValue = value;
			o.Call(Flow.New);
		}

		public override void OnGraphStoped(){

			if (string.IsNullOrEmpty(eventName)){
				return;
			}

			if (unityEvent != null){
				unityEvent.RemoveListener(Raised);
			}
		}

		protected override void RegisterPorts(){
			if (!string.IsNullOrEmpty(eventName)){
				o = AddFlowOutput(eventName);
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
				var menu = new UnityEditor.GenericMenu();
				foreach (var t in UserTypePrefs.GetPreferedTypesList(typeof(object))){
					menu = EditorUtils.GetStaticFieldSelectionMenu(t, typeof(UnityEvent<T>), SetEvent, menu);
				}
				menu.ShowAsContext();
				Event.current.Use();
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