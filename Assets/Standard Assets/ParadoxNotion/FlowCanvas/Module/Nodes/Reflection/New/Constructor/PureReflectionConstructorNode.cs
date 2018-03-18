using System.Reflection;

namespace FlowCanvas.Nodes
{
    public class PureReflectionConstructorNode : BaseReflectedConstructorNode
    {
        private object resultObject;
        private object[] callParams;
        private ValueInput[] inputs;

        protected override bool InitInternal(ConstructorInfo constructor)
        {
            callParams = new object[paramDefinitions.Count];
            inputs = new ValueInput[paramDefinitions.Count];
            resultObject = null;
            //allways can init =)
            return true;
        }

        private void Call()
        {
            for (int i = 0; i <= callParams.Length - 1; i++)
            {
                if (inputs[i] != null)
                {
                    callParams[i] = inputs[i].value;
                }
            }
            resultObject = constructorInfo.Invoke(callParams);
        }

        private void RegisterOutput(FlowNode node, bool callable, ParamDef def, int idx)
        {
            node.AddValueOutput(def.portName, def.paramType, () =>
            {
                if (!callable) Call();
                return callParams[idx];
            }, def.portId);
        }

        private void RegisterInput(FlowNode node, ParamDef def, int idx)
        {
            inputs[idx] = node.AddValueInput(def.portName, def.paramType, def.portId);
        }

        public override void RegisterPorts(FlowNode node, bool callable)
        {
            if (callable)
            {
                var output = node.AddFlowOutput(" ");
                node.AddFlowInput(" ", flow =>
                {
                    Call();
                    output.Call(flow);
                });
            }
            for (int i = 0; i <= paramDefinitions.Count - 1; i++)
            {
                var def = paramDefinitions[i];
                if (def.paramMode == ParamMode.Ref)
                {
                    RegisterInput(node, def, i);
                    RegisterOutput(node, callable, def, i);
                }
                else if (def.paramMode == ParamMode.In)
                {
                    RegisterInput(node, def, i);
                }
                else if (def.paramMode == ParamMode.Out)
                {
                    RegisterOutput(node, callable, def, i);
                }
            }
            if (resultDef.paramMode != ParamMode.Undefined)
            {
                node.AddValueOutput(resultDef.portName, resultDef.paramType, () =>
                {
                    if (!callable) Call();
                    return resultObject;
                }, resultDef.portId);
            }
        }
    }
}