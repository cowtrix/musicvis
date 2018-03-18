﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace FlowCanvas.Nodes
{
    public abstract class BaseReflectedMethodNode
    {
        protected static event Func<MethodInfo, BaseReflectedMethodNode> OnGetAotReflectedMethodNode;

        public static BaseReflectedMethodNode GetMethodNode(MethodInfo targetMethod)
        {
            ParametresDef paramDef;
            if (!ReflectedNodesHelper.InitParams(targetMethod, out paramDef)) return null;
#if UNITY_EDITOR || (!ENABLE_IL2CPP && (UNITY_STANDALONE || UNITY_ANDROID || UNITY_WSA))
            var jit = new JitMethodNode();
            if (jit.Init(targetMethod, paramDef))
            {
                return jit;
            }
#endif
            if (OnGetAotReflectedMethodNode != null)
            {
                var eventAot = OnGetAotReflectedMethodNode(targetMethod);
                if (eventAot != null && eventAot.Init(targetMethod, paramDef))
                {
                    return eventAot;
                }
            }
            var aot = new PureReflectedMethodNode();
            return aot.Init(targetMethod, paramDef) ? aot : null;
        }

        protected MethodInfo methodInfo;
        protected List<ParamDef> paramDefinitions;
        protected ParamDef instanceDef;
        protected ParamDef resultDef;

        protected bool Init(MethodInfo method, ParametresDef parametres)
        {
            if (method == null || method.ContainsGenericParameters || method.IsGenericMethodDefinition) return false;
            paramDefinitions = parametres.paramDefinitions == null?new List<ParamDef>():parametres.paramDefinitions;
            instanceDef = parametres.instanceDef;
            resultDef = parametres.resultDef;
            methodInfo = method;
            return InitInternal(method);
        }

        protected abstract bool InitInternal(MethodInfo method);

        public abstract void RegisterPorts(FlowNode node, bool callable);
    }
}