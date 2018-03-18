using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace FlowCanvas.Nodes{

	[Name("Visibility")]
	[Category("Events/Object")]
	[Description("Calls events based on object's render visibility")]
	public class VisibilityEvents : MessageEventNode<Transform> {

		private FlowOutput onVisible;
		private FlowOutput onInvisible;
		private GameObject sender;


		protected override string[] GetTargetMessageEvents(){
			return new string[]{ "OnBecameVisible", "OnBecameInvisible" };
		}

		protected override void RegisterPorts(){
			onVisible = AddFlowOutput("Became Visible");
			onInvisible = AddFlowOutput("Became Invisible");
			AddValueOutput<GameObject>("This", ()=>{ return sender; });
		}

		void OnBecameVisible(ParadoxNotion.Services.MessageRouter.MessageData msg){
			this.sender = ResolveSender(msg.sender).gameObject;
			onVisible.Call(new Flow(1));
		}

		void OnBecameInvisible(ParadoxNotion.Services.MessageRouter.MessageData msg){
			this.sender = ResolveSender(msg.sender).gameObject;
			onInvisible.Call(new Flow(1));
		}
	}
}