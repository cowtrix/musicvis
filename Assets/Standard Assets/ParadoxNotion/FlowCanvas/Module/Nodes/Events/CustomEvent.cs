using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using UnityEngine;

namespace FlowCanvas.Nodes{

	[Description("Called when a custom event is received on target.\n-To send an event from code use:\n'FlowScriptController.SendEvent(string)'\n- To send an event from a graph use the SendEvent node.")]
	[Category("Events/Custom")]
	public class CustomEvent : MessageEventNode<GraphOwner> {

		[RequiredField]
		public string eventName;

		private FlowOutput received;
		private GraphOwner sender;

		public override string name{
			get {return base.name + string.Format(" [ <color=#DDDDDD>{0}</color> ]", eventName); }
		}

		protected override string[] GetTargetMessageEvents(){
			return new string[]{ "OnCustomEvent" };
		}

		protected override void RegisterPorts(){
			received = AddFlowOutput("Received");
			AddValueOutput<GraphOwner>("This", ()=>{ return sender; });
		}

		public void OnCustomEvent(ParadoxNotion.Services.MessageRouter.MessageData<EventData> msg){
			if (msg.value.name == eventName){
				this.sender = ResolveSender(msg.sender);
				
				#if UNITY_EDITOR
				if (NodeCanvas.Editor.NCPrefs.logEvents){
					Debug.Log(string.Format("<b>Event Received from ({0}): </b> '{1}'", sender.name, msg.value.name), sender);
				}
				#endif

				received.Call(new Flow(1));
			}
		}
	}


	[Description("Called when a custom value-based event is received on target.\n-To send an event from code use:\n'FlowScriptController.SendEvent<T>(string name, T value)'\n-To send an event from a graph use the SendEvent<T> node.")]
	[Category("Events/Custom")]
	public class CustomEvent<T> : MessageEventNode<GraphOwner> {

		[RequiredField]
		public string eventName;
		private FlowOutput received;
		private T receivedValue;
		private GraphOwner sender;

		public override string name{
			get {return base.name + string.Format(" [ <color=#DDDDDD>{0}</color> ]", eventName); }
		}

		protected override string[] GetTargetMessageEvents(){
			return new string[]{ "OnCustomEvent" };
		}

		protected override void RegisterPorts(){
			received = AddFlowOutput("Received");
			AddValueOutput<T>("Event Value", ()=> { return receivedValue; });
			AddValueOutput<GraphOwner>("This", ()=>{ return sender; });
		}

		public void OnCustomEvent(ParadoxNotion.Services.MessageRouter.MessageData<EventData> msg){
			if (msg.value.name == eventName){
				this.sender = ResolveSender(msg.sender);
				if (msg.value is EventData<T>){
					receivedValue = (msg.value as EventData<T>).value;
				}

				#if UNITY_EDITOR
				if (NodeCanvas.Editor.NCPrefs.logEvents){
					Debug.Log(string.Format("<b>Event Received from ({0}): </b> '{1}'", sender.name, msg.value.name), sender);
				}
				#endif

				received.Call(new Flow(1));
			}
		}
	}

}