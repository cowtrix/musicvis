#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NodeCanvas.Framework;
using NodeCanvas;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;

namespace FlowCanvas{

	public interface IMultiPortNode{
		int portCount{get;set;}
	}

	///The base node class for FlowGraph systems
	abstract public class FlowNode : Node, ISerializationCallbackReceiver {
		
		[AttributeUsage(AttributeTargets.Class)]//helper attribute for context menu
		public class ContextDefinedInputsAttribute : Attribute{
			public Type[] types;
			public ContextDefinedInputsAttribute(params Type[] types){
				this.types = types;
			}
		}

		[AttributeUsage(AttributeTargets.Class)]//helper attribute for context menu
		public class ContextDefinedOutputsAttribute : Attribute{
			public Type[] types;
			public ContextDefinedOutputsAttribute(params Type[] types){
				this.types = types;
			}
		}


		[SerializeField] //The only thing we really need to serialize port wise, are the input port values that has been set by the user
		private Dictionary<string, object> _inputPortValues;

		private Dictionary<string, Port> inputPorts  = new Dictionary<string, Port>( StringComparer.Ordinal );
		private Dictionary<string, Port> outputPorts = new Dictionary<string, Port>( StringComparer.Ordinal );

		sealed public override int maxInConnections{ get {return -1;} }
		sealed public override int maxOutConnections{ get {return -1;} }
		sealed public override bool allowAsPrime{ get {return false;} }
		sealed public override Type outConnectionType{ get {return typeof(BinderConnection);} }
		sealed public override Alignment2x2 commentsAlignment{ get{return Alignment2x2.Bottom;}}
		public override Alignment2x2 iconAlignment{ get {return Alignment2x2.Left;} }

		///Store the changed input port values.
		void ISerializationCallbackReceiver.OnBeforeSerialize(){
			_inputPortValues = new Dictionary<string, object>();
			foreach (var port in inputPorts.Values.OfType<ValueInput>()){
				if (!port.isConnected && !port.isDefaultValue){
					_inputPortValues[port.ID] = port.serializedValue;
				}
			}
		}


		//Nothing... Instead, deserialize input port value AFTER GatherPorts and Validation. We DONT want to call GatherPorts here.
		void ISerializationCallbackReceiver.OnAfterDeserialize(){}


		///This is called when the node is created, duplicated or otherwise needs validation
		sealed public override void OnValidate(Graph flowGraph){
			GatherPorts();
		}

		//Callback on connection
		sealed public override void OnParentConnected(int i){
			if (i < inConnections.Count){
				var connection = inConnections[i] as BinderConnection;
				if (connection != null) {
					HandleWildPortChange(connection.targetPort, connection.sourcePort, this.GetType(), null);
					OnInputPortConnected(connection.targetPort, connection.sourcePort);
				}
			}
		}
		
		//Callback on connection
		sealed public override void OnChildConnected(int i){
			if (i < outConnections.Count){
				var connection = outConnections[i] as BinderConnection;
				if (connection != null) {
					HandleWildPortChange(connection.sourcePort, connection.targetPort, this.GetType(), null);					
					OnOutputPortConnected(connection.sourcePort, connection.targetPort);
				}
			}
		}

		//Callback on connection
		sealed public override void OnParentDisconnected(int i){
			if (i < inConnections.Count){
				var connection = inConnections[i] as BinderConnection;
				if (connection != null) { OnInputPortDisconnected(connection.targetPort, connection.sourcePort); }
			}
		}

		//Callback on connection
		sealed public override void OnChildDisconnected(int i){
			if (i < outConnections.Count){
				var connection = outConnections[i] as BinderConnection;
				if (connection != null) { OnOutputPortDisconnected(connection.sourcePort, connection.targetPort); }
			}			
		}

		///---------------------------------------------------------------------------------------------

		virtual public void OnInputPortConnected(Port port, Port otherPort){}
		virtual public void OnOutputPortConnected(Port port, Port otherPort){}
		virtual public void OnInputPortDisconnected(Port port, Port otherPort){}
		virtual public void OnOutputPortDisconnected(Port port, Port otherPort){}

		///---------------------------------------------------------------------------------------------

		///Bind the port delegates. Called at runtime
		public void BindPorts(){
			for (var i = 0; i < outConnections.Count; i++){
				(outConnections[i] as BinderConnection).Bind();
			}
		}

		///Unbind the ports
		public void UnBindPorts(){
			for (var i = 0; i < outConnections.Count; i++){
				(outConnections[i] as BinderConnection).UnBind();
			}
		}

		///Gets an input Port by it's ID name which commonly is the same as it's name
		public Port GetInputPort(string ID){
			Port input = null;
			inputPorts.TryGetValue(ID, out input);
			return input;
		}

		///Gets an output Port by it's ID name which commonly is the same as it's name
		public Port GetOutputPort(string ID){
			Port output = null;
			outputPorts.TryGetValue(ID, out output);
			return output;
		}

		///Gets the BinderConnection of an Input port based on it's ID
		public BinderConnection GetInputConnectionForPortID(string ID){
			return inConnections.OfType<BinderConnection>().FirstOrDefault(c => c.targetPortID == ID);
		}

		///Gets the BinderConnection of an Output port based on it's ID
		public BinderConnection GetOutputConnectionForPortID(string ID){
			return outConnections.OfType<BinderConnection>().FirstOrDefault(c => c.sourcePortID == ID);
		}

		///Returns the first input port assignable to the type provided
		public Port GetFirstInputOfType(Type type){
			return inputPorts.Values.OrderBy(p => p is FlowInput? 0 : 1 ).FirstOrDefault(p => p.type.RTIsAssignableFrom(type) );
		}

		///Returns the first output port of a type assignable to the port
		public Port GetFirstOutputOfType(Type type){
			return outputPorts.Values.OrderBy(p => p is FlowInput? 0 : 1 ).FirstOrDefault(p => type.RTIsAssignableFrom(p.type) );
		}

		
		///Set the Component or GameObject instance input port to Owner if not connected and not already set to another reference.
		///By convention, the instance port is always considered to be the first.
		//Called from Graph when started.
		public void AssignSelfInstancePort(){
			var instanceInput = inputPorts.Values.OfType<ValueInput>().FirstOrDefault();
			if (instanceInput != null && !instanceInput.isConnected && instanceInput.isDefaultValue){
				var instance = (graph as FlowGraph).GetAgentComponent(instanceInput.type);
				if (instance != null){
					instanceInput.serializedValue = instance;
				}
			}
		}

		///Gather and register the node ports.
		public void GatherPorts(){
			inputPorts.Clear();
			outputPorts.Clear();
			RegisterPorts();

			#if UNITY_EDITOR
			OnPortsGatheredInEditor();
			#endif

			DeserializeInputPortValues();
			
			#if UNITY_EDITOR
			ValidateConnections();
			#endif
		}

		///*IMPORTANT*
		///Override for registration/definition of ports.
		///If RegisterPorts is not overriden, reflection is used to register ports based on guidelines.
		///It's MUCH BETTER AND FASTER if you use the API for port registration instead though!
		virtual protected void RegisterPorts(){
			TryReflectionBasedRegistration(this);
		}

		///Reflection based registration if RegisterPorts is not overriden. Nowhere used by default in FC.
		void TryReflectionBasedRegistration(object instance){

			//FlowInputs. All void methods with one Flow parameter.
			foreach (var method in instance.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)){
				TryAddMethodFlowInput(method, instance);
			}

			//ValueOutputs. All readable public properties.
			foreach (var prop in instance.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)){
				TryAddPropertyValueOutput(prop, instance);
			}

			//Search for delegates fields
			foreach (var field in instance.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)){
				TryAddFieldDelegateFlowOutput(field, instance);
				TryAddFieldDelegateValueInput(field, instance);
			}
		}


		//Validate ports for connections
		//This is done seperately for Source and Target since we don't get control of when GatherPorts will be called on each node apart from in order of list (and we dont care)
		//So basicaly each node validates it's own inputs and outputs seperately.
		void ValidateConnections(){

			foreach (var cOut in outConnections.ToArray()){ //ToArray because connection might remove itself if invalid
				if (cOut is BinderConnection){
					(cOut as BinderConnection).GatherAndValidateSourcePort();
				}
			}

			foreach (var cIn in inConnections.ToArray()){
				if (cIn is BinderConnection){
					(cIn as BinderConnection).GatherAndValidateTargetPort();
				}
			}
		}


		///Restore the serialized input port values
		void DeserializeInputPortValues(){

			if (_inputPortValues == null){
				return;
			}

			foreach (var pair in _inputPortValues){
				Port inputPort = null;
				if ( inputPorts.TryGetValue(pair.Key, out inputPort)){
					if (inputPort is ValueInput && pair.Value != null && inputPort.type.RTIsAssignableFrom(pair.Value.GetType())){
						(inputPort as ValueInput).serializedValue = pair.Value;
					}
				}
			}
		}


		///---------------------------------------------------------------------------------------------
		//Port registration/definition methods, to be used within RegisterPorts override

		///Add a new FlowInput with name and pointer. Pointer is the method to run when the flow port is called. Returns the new FlowInput object.
		public FlowInput AddFlowInput(string name, string ID, FlowHandler pointer){ return AddFlowInput(name, pointer, ID); }
		public FlowInput AddFlowInput(string name, FlowHandler pointer, string ID = ""){
			if (string.IsNullOrEmpty(ID)) ID = name;
			return (FlowInput) (inputPorts[ID] = new FlowInput(this, name, ID, pointer) );
		}

		///Add a new FlowOutput with name. Returns the new FlowOutput object.
		public FlowOutput AddFlowOutput(string name, string ID = ""){
			if (string.IsNullOrEmpty(ID)) ID = name;
			return (FlowOutput) (outputPorts[ID] = new FlowOutput(this, name, ID) );
		}

		///Recommended. Add a ValueInput of type T. Returns the new ValueInput<T> object.
		public ValueInput<T> AddValueInput<T>(string name, string ID = ""){
			if (string.IsNullOrEmpty(ID)) ID = name;
			return (ValueInput<T>) (inputPorts[ID] = new ValueInput<T>(this, name, ID) );
		}

		///Recommended. Add a ValueOutput of type T. getter is the function to get the value from. Returns the new ValueOutput<T> object.
		public ValueOutput<T> AddValueOutput<T>(string name, string ID, ValueHandler<T> getter){ return AddValueOutput<T>(name, getter, ID); }
		public ValueOutput<T> AddValueOutput<T>(string name, ValueHandler<T> getter, string ID = ""){
			if (string.IsNullOrEmpty(ID)) ID = name;
			return (ValueOutput<T>) (outputPorts[ID] = new ValueOutput<T>(this, name, ID, getter) );
		}

		///Add a WildInput port which is an object type forced to a specific type instead. It's very recommended to use the generic registration methods instead.
		public ValueInput AddValueInput(string name, Type type, string ID = ""){
			if (string.IsNullOrEmpty(ID)) ID = name;
			return (ValueInput) (inputPorts[ID] = ValueInput.CreateInstance(type, this, name, ID) );
		}
		
		///Add a new WildOutput of unkown runtime type (similar to WildInput). getter is a function to get the port value from. Returns the new ValueOutput object.
		public ValueOutput AddValueOutput(string name, string ID, Type type, ValueHandler<object> getter){ return AddValueOutput(name, type, getter, ID); }
		public ValueOutput AddValueOutput(string name, Type type, ValueHandler<object> getter, string ID = ""){
			if (string.IsNullOrEmpty(ID)) ID = name;
			return (ValueOutput) (outputPorts[ID] = ValueOutput.CreateInstance(type, this, name, ID, getter) );
		}

		///----------------------------------------------------------------------------------------------

		///Register a PropertyInfo as ValueOutput. Used only in reflection based registration.
		public ValueOutput TryAddPropertyValueOutput(PropertyInfo prop, object instance){

			if (!prop.CanRead){
				return null;
			}

			var nameAtt = prop.RTGetAttribute<NameAttribute>(false);
			var name = nameAtt != null? nameAtt.name : prop.Name.SplitCamelCase();
			var getterType = typeof(ValueHandler<>).RTMakeGenericType( prop.PropertyType );
			var getter = prop.RTGetGetMethod().RTCreateDelegate(getterType, instance);
			var portType = typeof(ValueOutput<>).RTMakeGenericType( prop.PropertyType );
			var port = (ValueOutput)Activator.CreateInstance(portType, new object[]{ this, name, name, getter });
			return (ValueOutput) (outputPorts[name] = port);
		}

		///Register a MethodInfo as FlowInput. Used only in reflection based registration.
		public FlowInput TryAddMethodFlowInput(MethodInfo method, object instance){
			var parameters = method.GetParameters();
			if (method.ReturnType == typeof(void) && parameters.Length == 1 && parameters[0].ParameterType == typeof(Flow)){
				var nameAtt = method.RTGetAttribute<NameAttribute>(false);
				var name = nameAtt != null? nameAtt.name : method.Name.SplitCamelCase();
				var pointer = method.RTCreateDelegate<FlowHandler>(instance);
				return AddFlowInput(name, pointer);
			}
			return null;
		}

		///Register a FieldInfo Delegate (FlowHandler) as FlowOutput. Used only in reflection based registration.
		public FlowOutput TryAddFieldDelegateFlowOutput(FieldInfo field, object instance){
			if ( typeof(Delegate).RTIsAssignableFrom(field.FieldType) ){
				var nameAtt = field.RTGetAttribute<NameAttribute>(false);
				var name = nameAtt != null? nameAtt.name : field.Name.SplitCamelCase();
				if (field.FieldType == typeof(FlowHandler)){
					var flowOut = AddFlowOutput(name);
					field.SetValue(instance, (FlowHandler)flowOut.Call);
					return flowOut;
				}
			}
			return null;
		}

		///Register a FieldInfo Delegate (ValueHandler<T>) as ValueInput. Used only in reflection based registration.
		public ValueInput TryAddFieldDelegateValueInput(FieldInfo field, object instance){
			if ( typeof(Delegate).RTIsAssignableFrom(field.FieldType) ){
				var nameAtt = field.RTGetAttribute<NameAttribute>(false);
				var name = nameAtt != null? nameAtt.name : field.Name.SplitCamelCase();
				var invokeMethod = field.FieldType.GetMethod("Invoke");
				var parameters = invokeMethod.GetParameters();
				if (invokeMethod.ReturnType != typeof(void) && parameters.Length == 0){
					var delType = invokeMethod.ReturnType;
					var portType = typeof(ValueInput<>).RTMakeGenericType( delType );
					var port = (ValueInput)Activator.CreateInstance(portType, new object[]{ instance, name, name });
					var getterType = typeof(ValueHandler<>).RTMakeGenericType( delType );
					var getter = port.GetType().GetMethod("get_value").RTCreateDelegate(getterType, port);
					field.SetValue(instance, getter);
					inputPorts[name] = port;
					return port;
				}
			}
			return null;
		}

		///----------------------------------------------------------------------------------------------


		///Replace with another type
		public FlowNode ReplaceWith(System.Type t){
			var newNode = graph.AddNode(t, this.nodePosition) as FlowNode;
			if (newNode == null) return null;
			foreach(var c in inConnections.ToArray()){
				c.SetTarget(newNode);
			}
			foreach(var c in outConnections.ToArray()){
				c.SetSource(newNode);
			}
			newNode._inputPortValues = this._inputPortValues.ToDictionary(x => x.Key, y => y.Value);
			graph.RemoveNode(this);
			newNode.GatherPorts();
			return (FlowNode)newNode;
		}

		///Handles connecting to a wild port and changing generic version to that new connection
		public void HandleWildPortChange(Port port, Port otherPort, Type content, Type context){
			if (content.IsGenericType){
				var args = content.GetGenericArguments();
				var arg1 = args.FirstOrDefault();
				if (arg1 != typeof(object) && arg1.IsGenericType){
					HandleWildPortChange(port, otherPort, arg1, content);
					return;
				}
				if (args.Length == 1 && arg1 == typeof(object)){ //T is object
					var otherPortElement = otherPort.type.GetEnumerableElementType();
					var portElement = port.type.GetEnumerableElementType();
					var elementMatch = otherPortElement != null && portElement != null;
					if (port.type == typeof(object) || portElement == typeof(object)){ //the port is object
						var newType = content.GetGenericTypeDefinition().MakeGenericType( elementMatch? otherPortElement : otherPort.type);
						if (context != null && context.IsGenericType){
							newType = context.GetGenericTypeDefinition().MakeGenericType(newType);
						}
						this.ReplaceWith(newType);
					}
				}
			}
		}

		///Alternative Exit Call by providing a FlowOutput (otherwise, use 'flowOutput.Call(Flow f)' directly)
		public void Call(FlowOutput port, Flow f){
			port.Call(f);
		}

		//...
		public void Fail(string error = null){
			status = Status.Failure;
			if (error != null){
				Debug.LogError(string.Format("<b>Flow Execution Error:</b> '{0}' - '{1}'", this.name, error), graph.agent);
			}
		}

		//...
		public void SetStatus(Status status){
			this.status = status;
		}

























		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR
		
		private static Port clickedPort;
		private static int dragDropMisses;
		private static GUIStyle _leftLabelStyle;
		private static GUIStyle _rightLabelStyle;

		private Port[] orderedInputs;
		private Port[] orderedOutputs;
		private ValueInput firstValuePort;

		//for input ports
		private static GUIStyle leftLabelStyle{
			get
			{
				if (_leftLabelStyle == null){
					_leftLabelStyle = new GUIStyle(GUI.skin.GetStyle("label"));
					_leftLabelStyle.alignment = TextAnchor.UpperLeft;
				}
				return _leftLabelStyle;
			}
		}

		//for output ports
		private static GUIStyle rightLabelStyle{
			get
			{
				if (_rightLabelStyle == null){
					_rightLabelStyle = new GUIStyle(GUI.skin.GetStyle("label"));
					_rightLabelStyle.alignment = TextAnchor.MiddleRight;
				}
				return _rightLabelStyle;
			}
		}

		//when gathering ports and we are in Unity Editor
		//gather the ordered inputs and outputs
		void OnPortsGatheredInEditor(){
			orderedInputs = inputPorts.Values.OrderBy(p => p is FlowInput? 0 : 1 ).ToArray();
			orderedOutputs = outputPorts.Values.OrderBy(p => p is FlowOutput && p.IsDelegate() ? 0 : 1 ).ToArray();
			firstValuePort = orderedInputs.OfType<ValueInput>().FirstOrDefault();
		}

		//Get all output Connections of a port. Used only for when removing
		BinderConnection[] GetOutPortConnections(Port port){
			return outConnections.Cast<BinderConnection>().Where(c => c.sourcePort == port ).ToArray();
		}

		//Get all input Connections of a port. Used only for when removing
		BinderConnection[] GetInPortConnections(Port port){
			return inConnections.Cast<BinderConnection>().Where(c => c.targetPort == port ).ToArray();
		}


		//Seal it...
		sealed protected override void DrawNodeConnections(Rect drawCanvas, bool fullDrawPass, Vector2 canvasMousePos, float zoomFactor){

			var e = Event.current;


			//Port container graphics
			if (fullDrawPass || drawCanvas.Overlaps(nodeRect)){
				GUI.Box(new Rect(nodeRect.x - 8, nodeRect.y + 2, 10, nodeRect.height), string.Empty, (GUIStyle)"nodePortContainer");
				GUI.Box(new Rect(nodeRect.xMax - 2, nodeRect.y + 2, 10, nodeRect.height), string.Empty, (GUIStyle)"nodePortContainer");
			}
			///


			if (fullDrawPass || drawCanvas.Overlaps(nodeRect)){

				var portRect = new Rect(0,0,10,10);

				//INPUT Ports
				if (orderedInputs != null){

					Port instancePort = null;

					for (var i = 0; i < orderedInputs.Length; i++){

						var port = orderedInputs[i];
						var canConnect = true;
						if ((port == clickedPort) ||
							(clickedPort is FlowInput || clickedPort is ValueInput) ||
							(port.isConnected && port is ValueInput) ||
							(clickedPort != null && clickedPort.parent == port.parent) ||
							(clickedPort != null && !TypeConverter.HasConvertion(clickedPort.type, port.type)) )
						{
							canConnect = false;
						}

						portRect.width = port.isConnected? 12:10;
						portRect.height = portRect.width;
						portRect.center = new Vector2(nodeRect.x - 5, port.pos.y);
						port.pos = portRect.center;

						//first gameobject or component port is considered to be the 'instance' port by convention
						if (port == firstValuePort){
							if ( port.IsUnitySceneObject() ){
								instancePort = port;
							}
						}

						//Port graphic
						if (clickedPort != null && !canConnect && clickedPort != port){
							GUI.color = new Color(1,1,1,0.3f);
						}
						GUI.Box(portRect, string.Empty, port.isConnected? (GUIStyle)"nodePortConnected" : (GUIStyle)"nodePortEmpty");
						GUI.color = Color.white;

						//Tooltip
						if (portRect.Contains(e.mousePosition)){
							
							var labelString = (canConnect || port.isConnected || port == clickedPort)? port.type.FriendlyName() : "Can't Connect Here";
							var size = GUI.skin.GetStyle("box").CalcSize(new GUIContent(labelString));
							var rect = new Rect(0, 0, size.x + 10, size.y + 5);
							rect.x = portRect.x - size.x - 10;
							rect.y = portRect.y - size.y/2;
							GUI.Box(rect, labelString);
						
						//Or value
						} else {

							if ( port is ValueInput && !port.isConnected){
								//Only these types are shown their value
								if ( port.type.IsValueType || port.type == typeof(Type) || port.type == typeof(string) || port.IsUnityObject() ){
									var value = (port as ValueInput).serializedValue;
									string labelString = null;
									if (!(port as ValueInput).isDefaultValue ){
										if (value is UnityEngine.Object && value as UnityEngine.Object != null){
											labelString = string.Format("<b><color=#66ff33>{0}</color></b>", (value as UnityEngine.Object).name);
										} else {
											labelString = value.ToStringAdvanced();
										}
									} else if ( port == instancePort ){
										var exists = true;
										if (graphAgent != null && typeof(Component).IsAssignableFrom(port.type) ){
											exists = graphAgent.GetComponent(port.type) != null;
										}
										var color = exists? "66ff33" : "ff3300";
										labelString = string.Format("<color=#{0}><b>♟ <i>Self</i></b></color>", color);
									} else {
										GUI.color = new Color(1,1,1,0.15f);
										labelString = value.ToStringAdvanced();
									}
									var size = GUI.skin.GetStyle("label").CalcSize(new GUIContent(labelString));
									var rect = new Rect(0, 0, size.x, size.y);
									rect.x = portRect.x - size.x - 5;
									rect.y = portRect.y - size.y * 0.3f; //*0.3? something's wrong here. FIX
									GUI.Label(rect, labelString);
									GUI.color = Color.white;
								}
							}
						}

						if (Graph.allowClick){
							//Right click removes connections
							if (port.isConnected && e.type == EventType.ContextClick && portRect.Contains(e.mousePosition)){
								foreach(var c in GetInPortConnections(port)){
									graph.RemoveConnection(c);
								}
								e.Use();
								return;
							}

							//Click initialize new drag & drop connection
							if (e.type == EventType.MouseDown && e.button == 0 && portRect.Contains(e.mousePosition)){
								if (port.CanAcceptConnections() ){
									dragDropMisses = 0;
									clickedPort = port;
									e.Use();
								}
							}

							//Drop on creates connection
							if (e.type == EventType.MouseUp && e.button == 0 && clickedPort != null){
								if (portRect.Contains(e.mousePosition) && port.CanAcceptConnections() ){
									BinderConnection.Create(clickedPort, port);
									clickedPort = null;
									e.Use();
								}
							}
						}

					}
				}

				//OUTPUT Ports
				if (orderedOutputs != null){

					for (var i = 0; i < orderedOutputs.Length; i++){

						var port = orderedOutputs[i];
						var canConnect = true;
						if ((port == clickedPort) ||
							(clickedPort is FlowOutput || clickedPort is ValueOutput) ||
							(port.isConnected && port is FlowOutput) ||
							(clickedPort != null && clickedPort.parent == port.parent) ||
							(clickedPort != null && !TypeConverter.HasConvertion(port.type, clickedPort.type)) )
						{
							canConnect = false;
						}

						portRect.width = port.isConnected? 12:10;
						portRect.height = portRect.width;
						portRect.center = new Vector2(nodeRect.xMax + 5, port.pos.y);
						port.pos = portRect.center;

						//Port graphic
						if (clickedPort != null && !canConnect && clickedPort != port){
							GUI.color = new Color(1,1,1,0.3f);
						}
						GUI.Box(portRect, string.Empty, port.isConnected? (GUIStyle)"nodePortConnected" : (GUIStyle)"nodePortEmpty");
						GUI.color = Color.white;

						//Tooltip
						if (portRect.Contains(e.mousePosition)){
							var labelString = (canConnect || port.isConnected || port == clickedPort)? port.type.FriendlyName() : "Can't Connect Here";
							var size = GUI.skin.GetStyle("label").CalcSize(new GUIContent(labelString));
							var rect = new Rect(0, 0, size.x + 10, size.y + 5);
							rect.x = portRect.x + 15;
							rect.y = portRect.y - portRect.height/2;
							GUI.Box(rect, labelString);
						}

						if (Graph.allowClick){
							//Right click removes connections
							if (e.type == EventType.ContextClick && portRect.Contains(e.mousePosition)){
								foreach(var c in GetOutPortConnections(port)){
									graph.RemoveConnection(c);
								}
								e.Use();
								return;
							}

							//Click initialize new drag & drop connection
							if (e.type == EventType.MouseDown && e.button == 0 && portRect.Contains(e.mousePosition)){
								if (port.CanAcceptConnections() ){
									dragDropMisses = 0;
									clickedPort = port;
									e.Use();
								}
							}

							//Drop on creates connection
							if (e.type == EventType.MouseUp && e.button == 0 && clickedPort != null){
								if (portRect.Contains(e.mousePosition) && port.CanAcceptConnections() ){
									BinderConnection.Create(port, clickedPort);
									clickedPort = null;
									e.Use();
								}
							}
						}
						
					}
				}
			}



			///ACCEPT CONNECTION
			if (clickedPort != null && e.type == EventType.MouseUp){

				///ON NODE
				if (nodeRect.Contains(e.mousePosition)){

					var cachePort = clickedPort;
					var menu = new GenericMenu();

					if (cachePort is ValueOutput || cachePort is FlowOutput){
						if (orderedInputs != null){
							foreach (var _port in orderedInputs.Where(p => p.CanAcceptConnections() && TypeConverter.HasConvertion(cachePort.type, p.type) )){
								var port = _port;
								menu.AddItem(new GUIContent(string.Format("To: '{0}'", port.name) ), false, ()=> { BinderConnection.Create(cachePort, port); } );
							}
						}
					} else {
						if (orderedOutputs != null){
							foreach (var _port in orderedOutputs.Where(p => p.CanAcceptConnections() && TypeConverter.HasConvertion(p.type, cachePort.type) )){
								var port = _port;
								menu.AddItem(new GUIContent(string.Format("From: '{0}'", port.name) ), false, ()=> { BinderConnection.Create(port, cachePort); } );
							}
						}
					} 

					//append menu items
					menu = OnDragAndDropPortContextMenu(menu, cachePort);

					//if there is only 1 option, just do it
					if (menu.GetItemCount() == 1){
						EditorUtils.GetMenuItems(menu)[0].func();
					} else {
						Graph.PostGUI += ()=> { menu.ShowAsContext(); };
					}

					clickedPort = null;
					e.Use();
					///

				///ON CANVAS
				} else {

					dragDropMisses ++;
					if (dragDropMisses == graph.allNodes.Count && clickedPort != null){
						var cachePort = clickedPort;
						clickedPort = null;
						DoContextPortConnectionMenu(cachePort, e.mousePosition, zoomFactor);
						e.Use();
					}
				}
			}



			//Temp connection line when linking
			if (clickedPort != null && clickedPort.parent == this){
				var from = clickedPort.pos;
				var to = e.mousePosition;
				var xDiff = (from.x - to.x) * 0.8f;
				xDiff = to.x > from.x? xDiff : -xDiff;
				var tangA = clickedPort is FlowInput || clickedPort is ValueInput? new Vector2(xDiff, 0) : new Vector2(-xDiff, 0);
				var tangB = tangA * -1;
				Handles.DrawBezier(from, to, from + tangA , to + tangB, new Color(0.5f,0.5f,0.8f,0.8f), null, 3);
			}

			//Actualy draw the existing connections
			for (var i = 0; i < outConnections.Count; i++){
				var binder = outConnections[i] as BinderConnection;
				if (binder != null){ //for in case it's MissingConnection
					var sourcePort = binder.sourcePort;
					var targetPort = binder.targetPort;
					if (sourcePort != null && targetPort != null){
						if (fullDrawPass || drawCanvas.Overlaps(RectUtils.GetBoundRect(sourcePort.pos, targetPort.pos) ) ){
							binder.DrawConnectionGUI(sourcePort.pos, targetPort.pos);
						}
					}
				}
			}
		}


		///Let nodes handle ports draged on top of them
		virtual protected GenericMenu OnDragAndDropPortContextMenu(GenericMenu menu, Port port){
			return menu;
		}

		///Context menu for when dragging a connection on empty canvas
		void DoContextPortConnectionMenu(Port clickedPort, Vector2 mousePos, float zoomFactor){

			GenericMenu menu = new GenericMenu();
			
			var flowGraph = (FlowGraph)graph;
			if (clickedPort is ValueInput || clickedPort is ValueOutput){
				menu = flowGraph.AppendTypeReflectionNodesMenu(menu, clickedPort.type, "", mousePos, clickedPort, null);
			}
			menu = flowGraph.AppendFlowNodesMenu(menu, "", mousePos, clickedPort, null);
			menu = flowGraph.AppendSimplexNodesMenu(menu, "", mousePos, clickedPort, null);
			menu = flowGraph.AppedVariableNodesMenu(menu, "Variables", mousePos, clickedPort, null);
			menu = flowGraph.AppendAllReflectionNodesMenu(menu, "Reflected", mousePos, clickedPort, null);
			menu = flowGraph.AppendMacroNodesMenu(menu, "MACROS", mousePos, clickedPort, null);

			if (zoomFactor == 1 && NodeCanvas.Editor.NCPrefs.useBrowser){
				menu.ShowAsBrowser(string.Format("Add & Connect (Type of {0})", clickedPort.type.FriendlyName() ), graph.baseNodeType );
			} else {
				Graph.PostGUI += ()=> { menu.ShowAsContext(); };
			}
			Event.current.Use();

		}


		//Draw the port names in order
		protected override void OnNodeGUI(){

			GUILayout.BeginHorizontal();
			GUILayout.BeginVertical();
			if (orderedInputs != null){
				for (var i = 0; i < orderedInputs.Length; i++){
					var inPort = orderedInputs[i];
					if (inPort is FlowInput){
						GUILayout.Label(string.Format("<b>► {0}</b>", inPort.name), leftLabelStyle);
					} else {
						var color = UserTypePrefs.GetTypeColor(inPort.type);
						var hexColor = EditorUtils.ColorToHex(color);
						var text = string.Format("<color={0}>{1}{2}</color>", hexColor, inPort.IsEnumerableCollection()? "#" : string.Empty , inPort.name );
						var icon = NodeCanvas.Editor.NCPrefs.showIcons? UserTypePrefs.GetTypeIcon(inPort.type) : null;
						var tooltip = string.Empty; //TODO
						if (inPort.IsDelegate()){
							text = string.Format("<b><color={0}>[{1}]</color></b>", hexColor, text);
						}
						GUI.color = icon != null && icon.name == UserTypePrefs.DEFAULT_TYPE_ICON_NAME? color : Color.white;
						GUILayout.Label(new GUIContent(text, icon, tooltip), leftLabelStyle, GUILayout.MaxHeight(16));
						GUI.color = Color.white;
					}
					if (Event.current.type == EventType.Repaint){
						inPort.pos = new Vector2(inPort.pos.x, GUILayoutUtility.GetLastRect().center.y + nodeRect.y);
					}
				}
			}
			GUILayout.EndVertical();


			GUILayout.BeginVertical();
			if (orderedOutputs != null){
				for (var i = 0; i < orderedOutputs.Length; i++){
					var outPort = orderedOutputs[i];
					if (outPort is FlowOutput){
						GUILayout.Label(string.Format("<b>{0} ►</b>", outPort.name), rightLabelStyle);
					} else {
						var color = UserTypePrefs.GetTypeColor(outPort.type);
						var hexColor = EditorUtils.ColorToHex(color);
						var text = string.Format("<color={0}>{1}{2}</color>", hexColor, outPort.IsEnumerableCollection()? "#" : string.Empty, outPort.name);
						var icon = NodeCanvas.Editor.NCPrefs.showIcons? UserTypePrefs.GetTypeIcon(outPort.type) : null;
						var tooltip = string.Empty; //TODO
						if (outPort.IsDelegate()){
							text = string.Format("<b><color={0}>[{1}]</color></b>", hexColor, text);
						}
						GUI.color = icon != null && icon.name == UserTypePrefs.DEFAULT_TYPE_ICON_NAME? color : Color.white;
						GUILayout.Label(new GUIContent(text, icon, tooltip), rightLabelStyle, GUILayout.MaxHeight(16));
						GUI.color = Color.white;
					}
					if (Event.current.type == EventType.Repaint){
						outPort.pos = new Vector2(outPort.pos.x, GUILayoutUtility.GetLastRect().center.y + nodeRect.y);
					}
				}
			}
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			
		}

		//The inspector panel
		protected override void OnNodeInspectorGUI(){

			DrawDefaultInspector();

			if (this is IMultiPortNode){
				if (GUILayout.Button("Add Port")){
					((IMultiPortNode)this).portCount ++;
					GatherPorts();
				}

				GUI.enabled = ((IMultiPortNode)this).portCount > 1;
				if (GUILayout.Button("Remove Port")){
					var count = ((IMultiPortNode)this).portCount;
					count = Mathf.Max(1, count - 1);
					((IMultiPortNode)this).portCount = count;
					GatherPorts();
				}
				GUI.enabled = true;
			}

			EditorUtils.Separator();
			DrawValueInputsGUI();
		}

		//Show the serialized input port values if any
		protected void DrawValueInputsGUI(){

			foreach (var input in inputPorts.Values.OfType<ValueInput>() ){

				if (input.isConnected){
					EditorGUILayout.LabelField(input.name, "[CONNECTED]");
					continue;
				}

				input.serializedValue = EditorUtils.GenericField(input.name, input.serializedValue, input.type, null);
			}
		}


		//Override of right click node context menu
		protected override GenericMenu OnContextMenu(GenericMenu menu){
			menu = base.OnContextMenu(menu);
			if (outputPorts.Values.Any(p => p is FlowOutput)){ //breakpoints only work with FlowOutputs
				menu.AddItem(new GUIContent("Breakpoint"), isBreakpoint, ()=>{ isBreakpoint = !isBreakpoint; });
			}

			var type = this.GetType();
			if (type.IsGenericType){
				menu = EditorUtils.GetPreferedTypesSelectionMenu(type.GetGenericTypeDefinition(), (t)=>{ this.ReplaceWith(t); }, menu, "Change T Type");
			}

			return menu;
		}


		#endif
	}
}