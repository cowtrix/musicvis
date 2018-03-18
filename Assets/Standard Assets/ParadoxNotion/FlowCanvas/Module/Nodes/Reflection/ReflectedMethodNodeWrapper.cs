// #define USE_NEW_REFLECTION

using System.Collections.Generic;
using System.Reflection;
using ParadoxNotion;
using ParadoxNotion.Design;
using ParadoxNotion.Serialization;
using UnityEngine;
using FlowCanvas.Nodes.Legacy;
using System.Linq;

namespace FlowCanvas.Nodes
{
    ///Wraps a MethodInfo into a FlowGraph node
    [DoNotList]
    [Icon(runtimeIconTypeCallback:"GetRuntimeIconType")]
    public class ReflectedMethodNodeWrapper : FlowNode {

        System.Type GetRuntimeIconType(){
            return method != null? method.DeclaringType : null;
        }

        [SerializeField]
        private SerializedMethodInfo _method;
        [SerializeField]
        private bool _callable;
        [SerializeField]
        private bool _exposeParams;
        [SerializeField]
        private int _exposedParamsCount;

#if USE_NEW_REFLECTION
        private BaseReflectedMethodNode reflectedMethodNode{get;set;}
#else
        private ReflectedMethodNode reflectedMethodNode{get;set;}
#endif

        private MethodInfo method {
            get { return _method != null ? _method.Get() : null; }
        }

        public override string name {
            get
            {
                if (method != null){
                    if ( !method.IsStatic || method.IsExtensionMethod() ){
                        return method.Name.SplitCamelCase();
                    }
                    return string.Format("{0}.{1}", method.DeclaringType.FriendlyName(), method.Name.SplitCamelCase() );
                }
                if (_method != null) {
                    return string.Format("<color=#ff6457>* Missing Function *\n{0}</color>", _method.GetMethodString());
                }
                return "NOT SET";
            }
        }

#if UNITY_EDITOR
        public override string description {
            get { return method != null ? DocsByReflection.GetMemberSummary(method) : "Missing Method"; }
        }
#endif

        private bool callable {
            get { return _callable; }
            set
            {
                if (_callable != value) {
                    _callable = value;
                    GatherPorts();
                }
            }
        }

        private bool exposeParams{
            get {return _exposeParams;}
            set
            {
                if (_exposeParams != value){
                    _exposeParams = value;
                    _exposedParamsCount = 1;
                    GatherPorts();
                }
            }
        }

        private int exposedParamsCount{
            get {return _exposedParamsCount;}
            set
            {
                if (_exposedParamsCount != value){
                    _exposedParamsCount = value;
                    if (_exposedParamsCount <= 0){
                        _exposeParams = false;
                    }
                    GatherPorts();
                }
            }
        }

        ///Set a new MethodInfo to be used by ReflectedMethodNode
        public void SetMethod(MethodInfo newMethod, object instance = null){

            //drop hierarchy to base definition
            newMethod = newMethod.GetBaseDefinition();

            _method = new SerializedMethodInfo(newMethod);
            _callable = newMethod.ReturnType == typeof(void);
            GatherPorts();

            //set possible instance reference
            if (instance != null && !newMethod.IsStatic){
                var port = (ValueInput)GetFirstInputOfType(instance.GetType());
                if (port != null){
                    port.serializedValue = instance;
                }
            }

			//set default parameter values
			var parameters = newMethod.GetParameters();
			for (var i = 0; i < parameters.Length; i++){
				var parameter = parameters[i];
				if (parameter.IsOptional){
					var nameID = parameters[i].Name.SplitCamelCase();
                    var paramPort = GetInputPort(nameID) as ValueInput;
                    if (paramPort != null){
                        paramPort.serializedValue = parameter.DefaultValue;
                    }
				}
			}
        }

#if USE_NEW_REFLECTION
        
        ///Gather the ports through the wrapper
        protected override void RegisterPorts(){
            if (method == null){
                return;
            }
            reflectedMethodNode = BaseReflectedMethodNode.GetMethodNode(method);
            if (reflectedMethodNode != null){
                reflectedMethodNode.RegisterPorts(this, callable);
            }
        }
#else

		///Gather the ports through the wrapper
		protected override void RegisterPorts(){
			if (method == null){
				return;
			}
			reflectedMethodNode = ReflectedMethodNode.Create(method);
			if (reflectedMethodNode != null){
                var options = new ReflectedMethodRegistrationOptions();
                options.callable = callable;
                options.exposeParams = exposeParams;
                options.exposedParamsCount = exposedParamsCount;
				reflectedMethodNode.RegisterPorts(this, method, options);
			}
		}

#endif

        ////////////////////////////////////////
        ///////////GUI AND EDITOR STUFF/////////
        ////////////////////////////////////////
#if UNITY_EDITOR

        protected override UnityEditor.GenericMenu OnContextMenu(UnityEditor.GenericMenu menu){
			menu = base.OnContextMenu(menu);
			if (method != null){
				var overloads = new List<MethodInfo>();
				foreach(var m in method.ReflectedType.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)){
					if (m.Name == method.Name){
						overloads.Add(m);
					}
				}

				if (overloads.Count > 1){
					foreach(var _m in overloads){
						var m = _m;
						if (!m.IsGenericMethod){
							var isSame = m.SignatureName() == method.SignatureName();
							menu.AddItem( new GUIContent("Replace Method Overload/" + m.SignatureName()), isSame, ()=>{ SetMethod(m); } );
						}
					}
				}
			}
			return menu;
        }

        protected override void OnNodeInspectorGUI(){
            if (method != null) {

                GUILayout.BeginVertical("box");
                if (method.ReturnType != typeof(void) && !method.Name.StartsWith("get_")){
                    callable = UnityEditor.EditorGUILayout.Toggle("Callable", callable);
                }

#if USE_NEW_REFLECTION

                //....

#else
                var parameters = method.GetParameters();
                var lastParam = parameters.LastOrDefault();
                if (lastParam != null && lastParam.IsParams(parameters)){
                    exposeParams = UnityEditor.EditorGUILayout.Toggle("Expose Parameters", exposeParams);
                }
                if (exposeParams){
                    UnityEditor.EditorGUI.indentLevel++;
                    exposedParamsCount = UnityEditor.EditorGUILayout.DelayedIntField("Parameters Count", exposedParamsCount);
                    UnityEditor.EditorGUI.indentLevel--;
                }
                GUILayout.EndVertical();
#endif

            }

            if (method == null && _method != null){
                GUILayout.Label(_method.GetMethodString());
            }

            base.OnNodeInspectorGUI();
        }

#endif

    }
}