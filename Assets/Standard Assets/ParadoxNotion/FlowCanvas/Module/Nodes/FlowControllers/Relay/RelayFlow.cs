using System.Linq;
using ParadoxNotion.Design;
using UnityEngine;

namespace FlowCanvas.Nodes{

    [Description("Can be used with a RelayFlowOutput.")]
    [Category("Flow Controllers/Relay")]
    public class RelayFlowInput : FlowControlNode {

        [SerializeField]
        private string _sourceOutputUID;
        private string sourceOutputUID{
            get { return _sourceOutputUID; }
            set { _sourceOutputUID = value; }
        }

        private object _sourceOutput;
        private RelayFlowOutput sourceOutput{
            get
            {
                if (_sourceOutput == null){
                    _sourceOutput = graph.GetAllNodesOfType<RelayFlowOutput>().FirstOrDefault(i => i.UID == sourceOutputUID);
                    if (_sourceOutput == null){
                        _sourceOutput = new object();
                    }
                }
                return _sourceOutput as RelayFlowOutput;
            }
            set { _sourceOutput = value; }
        }

        public override string name{
            get { return string.Format("{0}", sourceOutput != null ? sourceOutput.ToString() : "@ NONE"); }
        }

        protected override void RegisterPorts(){
            AddFlowInput(" ", (f) => { if (sourceOutput != null && sourceOutput.port != null) sourceOutput.port.Call(f); });
        }

        ////////////////////////////////////////
        ///////////GUI AND EDITOR STUFF/////////
        ////////////////////////////////////////
        #if UNITY_EDITOR

        protected override void OnNodeInspectorGUI(){
            var relayInputs = graph.GetAllNodesOfType<RelayFlowOutput>();
            var currentInput = relayInputs.FirstOrDefault(i => i.UID == sourceOutputUID);
            var newInput = EditorUtils.Popup<RelayFlowOutput>("Relay Input Source", currentInput, relayInputs);
            if (newInput != currentInput){
                sourceOutputUID = newInput != null ? newInput.UID : null;
                sourceOutput = newInput != null ? newInput : null;
                GatherPorts();
            }
        }

        #endif
    }


    [Description("Can be used with a RelayFlowInput.")]
    [Category("Flow Controllers/Relay")]
    public class RelayFlowOutput : FlowControlNode {
        [Tooltip("The identifier name of the relay")]
        public string identifier = "MyFlowInputName";
        [HideInInspector]
        public FlowOutput port { get; private set; }

        public override string name{
            get { return string.Format("@ {0}", identifier); }
        }

        protected override void RegisterPorts(){
            port = AddFlowOutput(" ");
        }
    }
}