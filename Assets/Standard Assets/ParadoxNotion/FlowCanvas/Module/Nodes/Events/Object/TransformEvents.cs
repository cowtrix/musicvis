using NodeCanvas.Framework;
using ParadoxNotion.Design;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlowCanvas.Nodes{

	[Category("Events/Object")]
	[Description("Events relevant to transform changes")]
	public class TransformEvents : MessageEventNode<Transform> {

		private FlowOutput onParentChanged;
		private FlowOutput onChildrenChanged;
		private Transform sender;
		private Transform parent;
		private IEnumerable<Transform> children;

		protected override string[] GetTargetMessageEvents(){
			return new string[]{ "OnTransformParentChanged", "OnTransformChildrenChanged" };
		}

		protected override void RegisterPorts(){
			onParentChanged = AddFlowOutput("On Transform Parent Changed");
			onChildrenChanged = AddFlowOutput("On Transform Children Changed");
			AddValueOutput<Transform>("This", ()=>{ return sender; });
			AddValueOutput<Transform>("Parent", ()=>{ return parent; });
			AddValueOutput<IEnumerable<Transform>>("Children", ()=>{ return children; });
		}

		void OnTransformParentChanged(ParadoxNotion.Services.MessageRouter.MessageData msg){
			this.sender = ResolveSender(msg.sender);
			this.parent = sender.parent;
			this.children = sender.Cast<Transform>();
			onParentChanged.Call(new Flow(1));
		}

		void OnTransformChildrenChanged(ParadoxNotion.Services.MessageRouter.MessageData msg){
			this.sender = ResolveSender(msg.sender);
			this.parent = sender.parent;
			this.children = sender.Cast<Transform>();
			onChildrenChanged.Call(new Flow(1));
		}
	}
}