using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace FlowCanvas.Nodes{

	[Name("Trigger")]
	[Category("Events/Object")]
	[Description("Called when Trigger based event happen on target")]
	public class TriggerEvents : MessageEventNode<Collider> {

		private FlowOutput enter;
		private FlowOutput stay;
		private FlowOutput exit;
		private Collider sender;
		private GameObject other;


		protected override string[] GetTargetMessageEvents(){
			return new string[]{ "OnTriggerEnter", "OnTriggerStay", "OnTriggerExit" };
		}

		protected override void RegisterPorts(){
			enter = AddFlowOutput("Enter");
			stay = AddFlowOutput("Stay");
			exit = AddFlowOutput("Exit");
			AddValueOutput<Collider>("This", ()=> { return sender; });
			AddValueOutput<GameObject>("Other", ()=> { return other; });
		}

		void OnTriggerEnter(ParadoxNotion.Services.MessageRouter.MessageData<Collider> msg){
			this.sender = ResolveSender(msg.sender);
			this.other = msg.value.gameObject;
			enter.Call(new Flow(1));
		}

		void OnTriggerStay(ParadoxNotion.Services.MessageRouter.MessageData<Collider> msg){
			this.sender = ResolveSender(msg.sender);
			this.other = msg.value.gameObject;
			stay.Call(new Flow(1));
		}

		void OnTriggerExit(ParadoxNotion.Services.MessageRouter.MessageData<Collider> msg){
			this.sender = ResolveSender(msg.sender);
			this.other = msg.value.gameObject;
			exit.Call(new Flow(1));
		}
	}
}