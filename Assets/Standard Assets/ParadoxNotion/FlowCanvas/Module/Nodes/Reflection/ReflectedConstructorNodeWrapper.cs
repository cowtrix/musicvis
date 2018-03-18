using System.Collections.Generic;
using System.Reflection;
using ParadoxNotion;
using ParadoxNotion.Design;
using ParadoxNotion.Serialization;
using UnityEngine;

namespace FlowCanvas.Nodes
{
    ///Wraps a ConstructorInfo into a FlowGraph node
    [DoNotList]
    [Icon(runtimeIconTypeCallback:"GetRuntimeIconType")]
    public class ReflectedConstructorNodeWrapper : FlowNode{

		System.Type GetRuntimeIconType(){
			return constructor != null? constructor.DeclaringType : null;
		}

        [SerializeField]
        private SerializedConstructorInfo _constructor;
        [SerializeField]
        private bool _callable;

        private BaseReflectedConstructorNode reflectedConstructorNode { get; set; }

        private ConstructorInfo constructor {
            get { return _constructor != null ? _constructor.Get() : null; }
        }

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

        public override string name {
            get
            {
                if (constructor != null) {
                    return string.Format("New {0} ()", constructor.DeclaringType.FriendlyName());
                }
                if (_constructor != null) {
                    return string.Format("<color=#ff6457>* Missing Function *\n{0}</color>", _constructor.GetMethodString());
                }
                return "NOT SET";
            }
        }

#if UNITY_EDITOR
        public override string description {
            get { return constructor != null ? DocsByReflection.GetMemberSummary(constructor) : "Missing Constructor"; }
        }
#endif

        ///Set a new ConstructorInfo to be used by ReflectedConstructorNode
        public void SetConstructor(ConstructorInfo newConstructor) {
            _constructor = new SerializedConstructorInfo(newConstructor);
            GatherPorts();

			//set default parameter values
			var parameters = newConstructor.GetParameters();
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

        protected override void RegisterPorts() {
            if (constructor == null){
                return;
            }

            reflectedConstructorNode = BaseReflectedConstructorNode.GetConstructorNode(constructor);
            if (reflectedConstructorNode != null){
                reflectedConstructorNode.RegisterPorts(this, callable);
            }
        }

        ////////////////////////////////////////
        ///////////GUI AND EDITOR STUFF/////////
        ////////////////////////////////////////
#if UNITY_EDITOR

        protected override UnityEditor.GenericMenu OnContextMenu(UnityEditor.GenericMenu menu) {
            if (constructor != null && constructor.ReflectedType != null) {
                var alterConstructors = new List<ConstructorInfo>();
                foreach (var c in constructor.ReflectedType.GetConstructors(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)){
                    alterConstructors.Add(c);
                }

                if (alterConstructors.Count > 1){
                    foreach (var _c in alterConstructors){
                        var c = _c;
                        var isSame = c.SignatureName() == constructor.SignatureName();
                        menu.AddItem(new GUIContent("Replace Constructor Overload/" + c.SignatureName()), isSame, () => { SetConstructor(c); });
                    }
                }
            }
            return menu;
        }

        protected override void OnNodeInspectorGUI(){
            if (constructor != null){
                callable = UnityEditor.EditorGUILayout.Toggle("Callable", callable);
            }

            if (constructor == null && _constructor != null){
                GUILayout.Label(_constructor.GetMethodString());
            }

            base.OnNodeInspectorGUI();
        }
#endif

    }
}