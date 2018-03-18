using ParadoxNotion.Design;
using UnityEngine;


namespace FlowCanvas.Nodes{

	[Name("Trigger2D")]
	[Category("Events/Object")]
	[Description("Called when 2D Trigger based event happen on target")]
	public class Trigger2DEvents : MessageEventNode<Collider2D> {

		private FlowOutput enter;
		private FlowOutput stay;
		private FlowOutput exit;
		private GameObject other;
		private Collider2D sender;

		protected override string[] GetTargetMessageEvents(){
			return new string[]{ "OnTriggerEnter2D", "OnTriggerStay2D", "OnTriggerExit2D" };
		}

		protected override void RegisterPorts(){
			enter = AddFlowOutput("Enter");
			stay = AddFlowOutput("Stay");
			exit = AddFlowOutput("Exit");
			AddValueOutput<Collider2D>("This", ()=>{ return sender; });
			AddValueOutput<GameObject>("Other", ()=> { return other; });
		}

		void OnTriggerEnter2D(ParadoxNotion.Services.MessageRouter.MessageData<Collider2D> msg){
			this.sender = ResolveSender(msg.sender);
			this.other = msg.value.gameObject;
			enter.Call(new Flow(1));
		}

		void OnTriggerStay2D(ParadoxNotion.Services.MessageRouter.MessageData<Collider2D> msg){
			this.sender = ResolveSender(msg.sender);
			this.other = msg.value.gameObject;
			stay.Call(new Flow(1));
		}

		void OnTriggerExit2D(ParadoxNotion.Services.MessageRouter.MessageData<Collider2D> msg){
			this.sender = ResolveSender(msg.sender);
			this.other = msg.value.gameObject;
			exit.Call(new Flow(1));
		}
	}
}