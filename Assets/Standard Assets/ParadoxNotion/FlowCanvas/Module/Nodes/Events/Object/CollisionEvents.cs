using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace FlowCanvas.Nodes{

	[Name("Collision")]
	[Category("Events/Object")]
	[Description("Called when Collision based events happen on target and expose collision information")]
	public class CollisionEvents : MessageEventNode<Collider> {

		private FlowOutput enter;
		private FlowOutput stay;
		private FlowOutput exit;
		private Collider sender;
		private Collision collision;

		protected override string[] GetTargetMessageEvents(){
			return new string[]{ "OnCollisionEnter", "OnCollisionStay", "OnCollisionExit" };
		}

		protected override void RegisterPorts(){
			enter = AddFlowOutput("Enter");
			stay = AddFlowOutput("Stay");
			exit = AddFlowOutput("Exit");
			AddValueOutput<Collider>("This", ()=> { return sender; });
			AddValueOutput<GameObject>("Other", ()=> { return collision.gameObject; });
			AddValueOutput<ContactPoint>("Contact Point", ()=> { return collision.contacts[0]; });
			AddValueOutput<Collision>("Collision Info", ()=> { return collision; });
		}

		void OnCollisionEnter(ParadoxNotion.Services.MessageRouter.MessageData<Collision> msg){
			this.sender = ResolveSender(msg.sender);
			this.collision = msg.value;
			enter.Call(new Flow(1));
		}

		void OnCollisionStay(ParadoxNotion.Services.MessageRouter.MessageData<Collision> msg){
			this.sender = ResolveSender(msg.sender);
			this.collision = msg.value;
			stay.Call(new Flow(1));
		}

		void OnCollisionExit(ParadoxNotion.Services.MessageRouter.MessageData<Collision> msg){
			this.sender = ResolveSender(msg.sender);
			this.collision = msg.value;
			exit.Call(new Flow(1));
		}
	}
}