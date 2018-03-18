using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace FlowCanvas.Nodes{

	[Name("Collision2D")]
	[Category("Events/Object")]
	[Description("Called when 2D Collision based events happen on target and expose collision information")]
	public class Collision2DEvents : MessageEventNode<Collider2D> {

		private FlowOutput enter;
		private FlowOutput stay;
		private FlowOutput exit;
		private Collider2D sender;
		private Collision2D collision;

		protected override string[] GetTargetMessageEvents(){
			return new string[]{ "OnCollisionEnter2D", "OnCollisionStay2D", "OnCollisionExit2D" };
		}

		protected override void RegisterPorts(){
			enter = AddFlowOutput("Enter");
			stay = AddFlowOutput("Stay");
			exit = AddFlowOutput("Exit");
			AddValueOutput<Collider2D>("This", ()=> { return sender; });
			AddValueOutput<GameObject>("Other", ()=> { return collision.gameObject; });
			AddValueOutput<ContactPoint2D>("Contact Point", ()=> { return collision.contacts[0]; });
			AddValueOutput<Collision2D>("Collision Info", ()=> { return collision; });
		}

		void OnCollisionEnter2D(ParadoxNotion.Services.MessageRouter.MessageData<Collision2D> msg){
			this.sender = ResolveSender(msg.sender);
			this.collision = msg.value;
			enter.Call(new Flow(1));
		}

		void OnCollisionStay2D(ParadoxNotion.Services.MessageRouter.MessageData<Collision2D> msg){
			this.sender = ResolveSender(msg.sender);
			this.collision = msg.value;
			stay.Call(new Flow(1));
		}

		void OnCollisionExit2D(ParadoxNotion.Services.MessageRouter.MessageData<Collision2D> msg){
			this.sender = ResolveSender(msg.sender);
			this.collision = msg.value;
			exit.Call(new Flow(1));
		}
	}
}