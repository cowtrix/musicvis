using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using UnityEngine;
using UnityEngine.Events;

namespace FlowCanvas.Nodes{

	[Category("Events/Custom")]
	public class CustomCallbackEvent : EventNode {

		[RequiredField]
		public string identifier;
		private FlowOutput received;

		public override string name{
			get {return base.name + string.Format(" [ <color=#DDDDDD>{0}</color> ]", identifier); }
		}

		protected override void RegisterPorts(){
			AddValueOutput<UnityAction>("Delegate", ()=>{ return Call; });
			received = AddFlowOutput("Received");
		}

		void Call(){
			received.Call(new Flow());
		}
	}
}