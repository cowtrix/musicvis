using System;
using System.Reflection;
using ParadoxNotion;

namespace FlowCanvas.Nodes
{
    public abstract class UniversalDelegateParam
    {
        public ParamDef paramDef;
        public abstract Type GetCurrentType();
        public abstract void RegisterAsInput(FlowNode node);
        public abstract void RegisterAsOutput(FlowNode node, Action beforeReturn);
        public abstract void SetFromInput();
        public abstract void SetFromValue(object value);
        public abstract FieldInfo ValueField { get; }
    }


    public class UniversalDelegateParam<T> : UniversalDelegateParam
    {
        public T value;
        private ValueInput<T> valueInput;
        // ReSharper disable once StaticMemberInGenericType
        private static FieldInfo _fieldInfo;

        //required for activator
        // ReSharper disable once EmptyConstructor
        public UniversalDelegateParam()
        {
        }

        public override Type GetCurrentType()
        {
            return typeof(T);
        }

        public override void RegisterAsInput(FlowNode node)
        {
            if (paramDef.paramMode == ParamMode.Instance || paramDef.paramMode == ParamMode.In ||
                paramDef.paramMode == ParamMode.Ref || paramDef.paramMode == ParamMode.Result)
            {
                valueInput = node.AddValueInput<T>(paramDef.portName, paramDef.portId);
            }
        }

        public override void RegisterAsOutput(FlowNode node, Action beforReturn)
        {
            if (paramDef.paramMode == ParamMode.Instance || paramDef.paramMode == ParamMode.Out ||
                paramDef.paramMode == ParamMode.Ref || paramDef.paramMode == ParamMode.Result)
            {
                ValueHandler<T> handler = () =>
                {
                    if (beforReturn != null) beforReturn();
                    return value;
                };
                node.AddValueOutput(paramDef.portName, handler, paramDef.portId);
            }
        }

        public override void SetFromInput()
        {
            if (valueInput != null) value = valueInput.value;
        }

        public override void SetFromValue(object newValue)
        {
            value = (T) newValue;
        }

        public override FieldInfo ValueField
        {
            get
            {
                if (_fieldInfo == null)
                {
                    _fieldInfo = GetType().RTGetField("value");
                }
                return _fieldInfo;
            }
        }
    }
}