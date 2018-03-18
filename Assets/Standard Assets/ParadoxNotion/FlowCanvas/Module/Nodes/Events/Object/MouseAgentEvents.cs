using ParadoxNotion.Design;
using NodeCanvas.Framework;
using UnityEngine;

namespace FlowCanvas.Nodes{

	[Name("Mouse")]
	[Category("Events/Object")]
	[Description("Called when mouse based operations happen on target collider")]
	public class MouseAgentEvents : MessageEventNode<Collider> {

		private FlowOutput enter;
		private FlowOutput over;
		private FlowOutput exit;
		private FlowOutput down;
		private FlowOutput up;
		private FlowOutput drag;
		
		private Collider sender;
		private RaycastHit hit;

		protected override string[] GetTargetMessageEvents(){
			return new string[]{"OnMouseEnter", "OnMouseOver", "OnMouseExit", "OnMouseDown", "OnMouseUp", "OnMouseDrag"};
		}

		protected override void RegisterPorts(){
			down  = AddFlowOutput("Down");
			up    = AddFlowOutput("Up");
			enter = AddFlowOutput("Enter");
			over  = AddFlowOutput("Over");
			exit  = AddFlowOutput("Exit");
			drag  = AddFlowOutput("Drag");
			AddValueOutput<Collider>("This", ()=>{ return sender; });
			AddValueOutput<RaycastHit>("Info", ()=>{ return hit; });
		}

		void OnMouseEnter(ParadoxNotion.Services.MessageRouter.MessageData msg){
			this.sender = ResolveSender(msg.sender);
			StoreHit();
			enter.Call(new Flow(1));
		}

		void OnMouseOver(ParadoxNotion.Services.MessageRouter.MessageData msg){
			this.sender = ResolveSender(msg.sender);
			StoreHit();
			over.Call(new Flow(1));
		}

		void OnMouseExit(ParadoxNotion.Services.MessageRouter.MessageData msg){
			this.sender = ResolveSender(msg.sender);
			StoreHit();
			exit.Call(new Flow(1));
		}

		void OnMouseDown(ParadoxNotion.Services.MessageRouter.MessageData msg){
			this.sender = ResolveSender(msg.sender);
			StoreHit();
			down.Call(new Flow(1));
		}

		void OnMouseUp(ParadoxNotion.Services.MessageRouter.MessageData msg){
			this.sender = ResolveSender(msg.sender);
			StoreHit();
			up.Call(new Flow(1));
		}

		void OnMouseDrag(ParadoxNotion.Services.MessageRouter.MessageData msg){
			this.sender = ResolveSender(msg.sender);
			StoreHit();
			drag.Call(new Flow(1));
		}

		void StoreHit(){
			Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity);
		}
	}
}