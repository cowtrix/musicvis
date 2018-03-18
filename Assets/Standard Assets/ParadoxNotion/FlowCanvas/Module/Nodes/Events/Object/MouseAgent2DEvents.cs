using ParadoxNotion.Design;
using NodeCanvas.Framework;
using UnityEngine;

namespace FlowCanvas.Nodes{

	[Name("Mouse2D")]
	[Category("Events/Object")]
	[Description("Called when mouse based operations happen on target 2D collider")]
	public class MouseAgent2DEvents : MessageEventNode<Collider2D> {

		private FlowOutput enter;
		private FlowOutput over;
		private FlowOutput exit;
		private FlowOutput down;
		private FlowOutput up;
		private FlowOutput drag;

		private Collider2D sender;
		private RaycastHit2D hit;

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
			AddValueOutput<Collider2D>("This", ()=>{ return sender; });
			AddValueOutput<RaycastHit2D>("Info", ()=>{ return hit; });
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
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
		}
	}
}