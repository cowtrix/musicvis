using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace FlowCanvas.Nodes{

	[Name("Mouse")]
	[Category("Events/Input")]
	[Description("Called when the specified mouse button is clicked down, held or released")]
	public class MouseEvents : EventNode, IUpdatable {

		public enum ButtonKeys
		{
			Left = 0,
			Right = 1,
			Middle = 2
		}

		public BBParameter<ButtonKeys> buttonKey;

		private FlowOutput down;
		private FlowOutput pressed;
		private FlowOutput up;

		public override string name{
			get {return string.Format("<color=#ff5c5c>➥ Mouse Button '{0}'</color>", buttonKey ).ToUpper();}
		}

		protected override void RegisterPorts(){
			down = AddFlowOutput("Down");
			pressed = AddFlowOutput("Pressed");
			up = AddFlowOutput("Up");
		}

		public void Update(){
			var value = (int)buttonKey.value;
			if (Input.GetMouseButtonDown(value)){
				down.Call(new Flow(1));
			}

			if (Input.GetMouseButton(value)){
				pressed.Call(new Flow(1));
			}

			if (Input.GetMouseButtonUp(value)){
				up.Call(new Flow(1));
			}
		}
	}
}