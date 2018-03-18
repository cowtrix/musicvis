using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using NodeCanvas.Tasks.Actions;
using ParadoxNotion;

namespace FlowCanvas.Nodes
{
    public static class ReflectedNodesHelper
    {
        public static ParamDef GetDefFromInfo(ParameterInfo info)
        {
            ParamDef result = new ParamDef();
            if (info != null)
            {
                var directType = info.ParameterType;
                var elementType = directType.GetElementType();
                var realType = directType.IsByRef && elementType != null ? elementType : directType;
                result.paramType = realType;
                if (info.IsOut && directType.IsByRef)
                {
                    result.paramMode = ParamMode.Out;
                }
                else if (!info.IsOut && info.ParameterType.IsByRef)
                {
                    result.paramMode = ParamMode.Ref;
                }
                else
                {
                    result.paramMode = ParamMode.In;
                }
                result.portName = info.Name.SplitCamelCase();
                result.portId = result.portName;
            }
            return result;
        }

        public static string GetGeneratedKey(MemberInfo memberInfo)
        {
            if (memberInfo != null)
            {
                return memberInfo.DeclaringType.FullName + " " + memberInfo.MemberType + " " + memberInfo;
            }
            return "";
        }

        private static bool InitParams(ParameterInfo[] prms, Type returnType, ref ParametresDef parametres)
        {
            bool useValueName = false;
            for (int i = 0; i <= prms.Length - 1; i++)
            {
                var def = GetDefFromInfo(prms[i]);
                if (def.portId == "Value" && !useValueName) useValueName = true;
                if (parametres.instanceDef.paramMode != ParamMode.Undefined && def.portId == parametres.instanceDef.portId &&
                    (def.paramMode == ParamMode.In || def.paramMode == ParamMode.Ref || def.paramMode == ParamMode.Out))
                {
                    def.portId += " ";
                }
                parametres.paramDefinitions.Add(def);
            }
            if (returnType != typeof(void))
            {
                parametres.resultDef.paramType = returnType;
                //TODO: ADD SOME SYMBOL
                parametres.resultDef.portName = "Value";
                parametres.resultDef.portId = useValueName ? "*Value" : "Value";
                parametres.resultDef.paramMode = ParamMode.Result;
            }
            return true;
        }

        public static bool InitParams(ConstructorInfo constructor, out ParametresDef parametres)
        {
            parametres = new ParametresDef
            {
                paramDefinitions = new List<ParamDef>(),
                instanceDef = new ParamDef { paramMode = ParamMode.Undefined },
                resultDef = new ParamDef { paramMode = ParamMode.Undefined }
            };
            if (constructor == null || constructor.ContainsGenericParameters || constructor.IsGenericMethodDefinition) return false;
            var prms = constructor.GetParameters();
            var returnType = constructor.RTReflectedType();
            return InitParams(prms, returnType, ref parametres);
        }

        public static bool InitParams(MethodInfo method, out ParametresDef parametres)
        {
            parametres = new ParametresDef
            {
                paramDefinitions = new List<ParamDef>(),
                instanceDef = new ParamDef { paramMode = ParamMode.Undefined },
                resultDef = new ParamDef { paramMode = ParamMode.Undefined }
            };
            if (method == null || method.ContainsGenericParameters || method.IsGenericMethodDefinition) return false;
            var prms = method.GetParameters();
            var returnType = method.ReturnType;
            if (!method.IsStatic)
            {
                parametres.instanceDef.paramType = method.DeclaringType;
                //TODO: ADD SOME SYMBOL
                parametres.instanceDef.portName = method.DeclaringType.FriendlyName();
                parametres.instanceDef.portId = method.DeclaringType.FriendlyName();
                parametres.instanceDef.paramMode = ParamMode.Instance;
            }
            return InitParams(prms, returnType, ref parametres);
        }

        public static bool InitParams(FieldInfo field, out ParametresDef parametres)
        {
            parametres = new ParametresDef
            {
                paramDefinitions = null,
                instanceDef = new ParamDef { paramMode = ParamMode.Undefined},
                resultDef = new ParamDef { paramMode = ParamMode.Undefined },
            };
            if (field == null || field.FieldType.ContainsGenericParameters || field.FieldType.IsGenericTypeDefinition) return false;
            if (!field.IsStatic)
            {
                parametres.instanceDef.paramMode = ParamMode.Instance;
                parametres.instanceDef.paramType = field.DeclaringType;
                parametres.instanceDef.portName = field.DeclaringType.FriendlyName();
                parametres.instanceDef.portId = field.DeclaringType.FriendlyName();
            }
            parametres.resultDef.paramMode = ParamMode.Result;
            parametres.resultDef.paramType = field.FieldType;
            parametres.resultDef.portName = "Value";
            parametres.resultDef.portId = "Value";
            return true;
        }

        public static object CreateObject(this Type type)
        {
            if(type == null) return null;
#if !UNITY_EDITOR && (UNITY_METRO || UNITY_WP8)
            return Activator.CreateInstance(type);
#else
            return FormatterServices.GetUninitializedObject(type);
#endif
        }
    }
}