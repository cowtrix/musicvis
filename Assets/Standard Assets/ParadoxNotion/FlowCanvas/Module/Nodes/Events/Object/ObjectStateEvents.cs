using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace FlowCanvas.Nodes{

	[Name("Object State")]
	[Category("Events/Object")]
	[Description("OnEnable, OnDisable and OnDestroy callback events for target object")]
	public class ObjectStateEvents : MessageEventNode<Transform> {

		private FlowOutput onEnable;
		private FlowOutput onDisable;
		private FlowOutput onDestroy;
		private GameObject sender;

		protected override string[] GetTargetMessageEvents(){
			return new string[]{ "OnEnable", "OnDisable", "OnDestroy" };
		}

		protected override void RegisterPorts(){
			onEnable = AddFlowOutput("On Enable");
			onDisable = AddFlowOutput("On Disable");
			onDestroy = AddFlowOutput("On Destroy");
			AddValueOutput<GameObject>("This", ()=>{ return sender; });
		}

		void OnEnable(ParadoxNotion.Services.MessageRouter.MessageData msg){
			this.sender = ResolveSender(msg.sender).gameObject;
			onEnable.Call(new Flow(1));
		}

		void OnDisable(ParadoxNotion.Services.MessageRouter.MessageData msg){
			this.sender = ResolveSender(msg.sender).gameObject;
			onDisable.Call(new Flow(1));
		}

		void OnDestroy(ParadoxNotion.Services.MessageRouter.MessageData msg){
			this.sender = ResolveSender(msg.sender).gameObject;
			onDestroy.Call(new Flow(1));
		}
	}
}