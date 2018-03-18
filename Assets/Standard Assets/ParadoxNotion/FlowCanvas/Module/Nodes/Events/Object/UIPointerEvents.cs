using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace FlowCanvas.Nodes{

	[Name("UI Pointer")]
	[Category("Events/Object")]
	[Description("Calls UI Pointer based events on target. The Unity Event system has to be set through 'GameObject/UI/Event System'")]
	public class UIPointerEvents : MessageEventNode<Transform> {

		private FlowOutput pointerDown;
		private FlowOutput pointerPressed;
		private FlowOutput pointerUp;
		private FlowOutput pointerEnter;
		private FlowOutput pointerExit;
		private FlowOutput pointerClick;
		private GameObject sender;
		private PointerEventData eventData;

		private bool updatePressed = false;

		protected override string[] GetTargetMessageEvents(){
			return new string[]{ "OnPointerEnter", "OnPointerExit", "OnPointerDown", "OnPointerUp", "OnPointerClick" };
		}

		protected override void RegisterPorts(){
			pointerClick = AddFlowOutput("Click");
			pointerDown  = AddFlowOutput("Down");
			pointerPressed= AddFlowOutput("Pressed");
			pointerUp    = AddFlowOutput("Up");
			pointerEnter = AddFlowOutput("Enter");
			pointerExit  = AddFlowOutput("Exit");
			AddValueOutput<GameObject>("This", ()=>{ return sender; });
			AddValueOutput<PointerEventData>("Event Data", ()=> { return eventData; });
		}

		void OnPointerDown(ParadoxNotion.Services.MessageRouter.MessageData<PointerEventData> msg){
			this.sender = ResolveSender(msg.sender).gameObject;
			this.eventData = msg.value;
			pointerDown.Call(new Flow(1));
			updatePressed = true;
			StartCoroutine(UpdatePressed());
		}

		void OnPointerUp(ParadoxNotion.Services.MessageRouter.MessageData<PointerEventData> msg){
			this.sender = ResolveSender(msg.sender).gameObject;
			this.eventData = msg.value;
			pointerUp.Call(new Flow(1));
			updatePressed = false;
		}


		IEnumerator UpdatePressed(){
			while(updatePressed){
				pointerPressed.Call(new Flow(1));
				yield return null;
			}
		}

		void OnPointerEnter(ParadoxNotion.Services.MessageRouter.MessageData<PointerEventData> msg){
			this.sender = ResolveSender(msg.sender).gameObject;
			this.eventData = msg.value;
			pointerEnter.Call(new Flow(1));
		}

		void OnPointerExit(ParadoxNotion.Services.MessageRouter.MessageData<PointerEventData> msg){
			this.sender = ResolveSender(msg.sender).gameObject;
			this.eventData = msg.value;
			pointerExit.Call(new Flow(1));
		}

		void OnPointerClick(ParadoxNotion.Services.MessageRouter.MessageData<PointerEventData> msg){
			this.sender = ResolveSender(msg.sender).gameObject;
			this.eventData = msg.value;
			pointerClick.Call(new Flow(1));
		}
	}
}