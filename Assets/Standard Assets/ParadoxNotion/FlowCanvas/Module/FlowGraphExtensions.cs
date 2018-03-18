#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Reflection;
using System.Linq;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using FlowCanvas.Nodes;
using FlowCanvas.Macros;


namespace FlowCanvas{

	public static class FlowGraphExtensions {

		//...
		public static FlowNode AddFlowNode(this FlowGraph graph, System.Type type, Vector2 pos, Port sourcePort, object dropInstance){
			var node = (FlowNode)graph.AddNode(type, pos);
			FinalizeConnection(sourcePort, node);
			DropInstance(node, dropInstance);
			return node;
		}

		//...
		public static FlowNode AddSimplexNode(this FlowGraph graph, System.Type t, Vector2 pos, Port sourcePort, object dropInstance){
			var genericType = typeof(SimplexNodeWrapper<>).MakeGenericType(new System.Type[]{ (System.Type)t });
			var wrapper = (FlowNode)graph.AddNode(genericType, pos);
			FinalizeConnection(sourcePort, wrapper);
			DropInstance(wrapper, dropInstance);
			return wrapper;
		}

		//...
		public static ReflectedConstructorNodeWrapper AddContructorNode(this FlowGraph graph, ConstructorInfo c, Vector2 pos, Port sourcePort, object dropInstance){
			var wrapper = graph.AddNode<ReflectedConstructorNodeWrapper>(pos);
			wrapper.SetConstructor(c);
			FinalizeConnection(sourcePort, wrapper);
			DropInstance(wrapper, dropInstance);
			return wrapper;
		}

		//...
		public static ReflectedMethodNodeWrapper AddMethodNode(this FlowGraph graph, MethodInfo m, Vector2 pos, Port sourcePort, object dropInstance){
			var wrapper = graph.AddNode<ReflectedMethodNodeWrapper>(pos);
			wrapper.SetMethod(m);
			FinalizeConnection(sourcePort, wrapper);
			DropInstance(wrapper, dropInstance);
			return wrapper;
		}

		//...
		public static ReflectedFieldNodeWrapper AddFieldGetNode(this FlowGraph graph, FieldInfo f, Vector2 pos, Port sourcePort, object dropInstance){
			var wrapper = graph.AddNode<ReflectedFieldNodeWrapper>(pos);
			wrapper.SetField( f, ReflectedFieldNodeWrapper.AccessMode.GetField );
			FinalizeConnection(sourcePort, wrapper);
			DropInstance(wrapper, dropInstance);
			return wrapper;
		}

		//...
		public static ReflectedFieldNodeWrapper AddFieldSetNode(this FlowGraph graph, FieldInfo f, Vector2 pos, Port sourcePort, object dropInstance){
			var wrapper = graph.AddNode<ReflectedFieldNodeWrapper>(pos);
			wrapper.SetField( f, ReflectedFieldNodeWrapper.AccessMode.SetField );			
			FinalizeConnection(sourcePort, wrapper);
			DropInstance(wrapper, dropInstance);
			return wrapper;
		}

		//...
		public static VariableNode AddVariableGet(this FlowGraph graph, System.Type varType, string varName, Vector2 pos, Port sourcePort, object dropInstance){
			var genericType = typeof(GetVariable<>).MakeGenericType(new System.Type[]{varType});
			var varNode = (VariableNode)graph.AddNode(genericType, pos);
			genericType.GetMethod("SetTargetVariableName").Invoke(varNode, new object[]{varName});			
			FinalizeConnection(sourcePort, varNode);
			DropInstance(varNode, dropInstance);
			if (dropInstance != null){
				varNode.SetVariable(dropInstance);
			}
			return varNode;
		}

		//...
		public static FlowNode AddVariableSet(this FlowGraph graph, System.Type varType, string varName, Vector2 pos, Port sourcePort, object dropInstance){
			var genericType = typeof(SetVariable<>).MakeGenericType(new System.Type[]{varType});
			var varNode = (FlowNode)graph.AddNode(genericType, pos);
			genericType.GetMethod("SetTargetVariableName").Invoke(varNode, new object[]{varName});
			FinalizeConnection(sourcePort, varNode);
			DropInstance(varNode, dropInstance);
			return varNode;
		}

		//...
		public static MacroNodeWrapper AddMacroNode(this FlowGraph graph, Macro m, Vector2 pos, Port sourcePort, object dropInstance){
			var wrapper = graph.AddNode<MacroNodeWrapper>(pos);
			wrapper.macro = (Macro)m;
			FinalizeConnection(sourcePort, wrapper);
			DropInstance(wrapper, dropInstance);
			return wrapper;
		}

		//...
		public static UnityEventNodeBase AddUnityEventNode(this FlowGraph graph, FieldInfo field, Vector2 pos, Port sourcePort, object dropInstance){
			var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy;
			var invokeMethod = field.FieldType.GetMethod("Invoke", flags);
			var parameters = invokeMethod.GetParameters();
			System.Type eventNodeType = null;
			if (parameters.Length == 0){ eventNodeType = typeof(UnityEventNode); }
			if (parameters.Length == 1){ eventNodeType = typeof(UnityEventNode<>).MakeGenericType(parameters[0].ParameterType); }
			if (eventNodeType != null){
				var wrapper = (UnityEventNodeBase)graph.AddNode(eventNodeType, pos);
				wrapper.SetEvent(field, dropInstance);
				FinalizeConnection(sourcePort, wrapper);
				DropInstance(wrapper, dropInstance);
				return wrapper;
			}
			return null;
		}

/*
		//...
		public static CodeEvent AddSharpEventNode(this FlowGraph graph, EventInfo e, Vector2 pos, Port sourcePort, object dropInstance){
			return null;
		}
*/

		//...
		public static CustomObjectWrapper AddObjectWrapper(this FlowGraph graph, System.Type type, Vector2 pos, Port sourcePort, UnityEngine.Object dropInstance){
			var wrapperNode = (CustomObjectWrapper)graph.AddNode(type, pos);
			wrapperNode.SetTarget(dropInstance);
			return wrapperNode;
		}

		///----------------------------------------------------------------------------------------------

		//...
		public static void FinalizeConnection(Port sourcePort, FlowNode targetNode){
			if (sourcePort == null || targetNode == null){
				return;
			}

			Port source = null;
			Port target = null;

			if (sourcePort is ValueOutput || sourcePort is FlowOutput){
				source = sourcePort;
				target = targetNode.GetFirstInputOfType(sourcePort.type);
			} else {
				source = targetNode.GetFirstOutputOfType(sourcePort.type);
				target = sourcePort;
			}

			BinderConnection.Create(source, target);
			Graph.currentSelection = targetNode;
		}

		//...
		public static void DropInstance(FlowNode targetNode, object dropInstance){
			if (targetNode == null || dropInstance == null){
				return;
			}
			var instancePort = targetNode.GetFirstInputOfType(dropInstance.GetType()) as ValueInput;
			if (instancePort != null){
				instancePort.serializedValue = dropInstance;
			}
		}

		///----------------------------------------------------------------------------------------------

		//FlowNode
		public static UnityEditor.GenericMenu AppendFlowNodesMenu(this FlowGraph graph, UnityEditor.GenericMenu menu, string baseCategory, Vector2 pos, Port sourcePort, object dropInstance){
			foreach(var _info in EditorUtils.GetScriptInfosOfType(typeof(FlowNode))){
				var info = _info;

				if (sourcePort != null){
					var definedInputTypesAtt = info.type.RTGetAttribute<FlowNode.ContextDefinedInputsAttribute>(true);
					var definedOutputTypesAtt = info.type.RTGetAttribute<FlowNode.ContextDefinedOutputsAttribute>(true);
					if (sourcePort is ValueOutput || sourcePort is FlowOutput){
						if (definedInputTypesAtt == null || !definedInputTypesAtt.types.Any(t => t.IsAssignableFrom(sourcePort.type))){
							continue;
						}
					}
					if (sourcePort is ValueInput || sourcePort is FlowInput){
						if (definedOutputTypesAtt == null || !definedOutputTypesAtt.types.Any(t => sourcePort.type.IsAssignableFrom(t) )){
							continue;
						}
					}
				}

				var category = string.Join("/", new string[]{ baseCategory, info.category, info.name }).TrimStart('/');
				menu.AddItem(new GUIContent(category, info.icon, info.description), false, ()=>{ graph.AddFlowNode(info.type, pos, sourcePort, dropInstance); });
			}
			return menu;
		}

		///All reflection type nodes
		private static List<System.Type> preferedTypesList;
		public static UnityEditor.GenericMenu AppendAllReflectionNodesMenu(this FlowGraph graph, UnityEditor.GenericMenu menu, string baseCategory, Vector2 pos, Port sourcePort, object dropInstance){
			preferedTypesList = UserTypePrefs.GetPreferedTypesList();
			foreach (var type in preferedTypesList){
				menu = graph.AppendTypeReflectionNodesMenu(menu, type, baseCategory, pos, sourcePort, dropInstance);
			}
			return menu;
		}

		///Refletion nodes on a type
		public static UnityEditor.GenericMenu AppendTypeReflectionNodesMenu(this FlowGraph graph, UnityEditor.GenericMenu menu, System.Type type, string baseCategory, Vector2 pos, Port sourcePort, object dropInstance){
			
			if (preferedTypesList == null){
				preferedTypesList = UserTypePrefs.GetPreferedTypesList();
			}

			if (!string.IsNullOrEmpty(baseCategory)){
				baseCategory += "/";
			}

			var icon = UserTypePrefs.GetTypeIcon(type);
			// var flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

			if (sourcePort is ValueInput){
				if (sourcePort.type == type){
					var portValue = (sourcePort as ValueInput).serializedValue;
					menu.AddItem(new GUIContent("Make Graph Variable", icon, null), false, (o)=>{ graph.AddVariableGet((System.Type)o, null, pos, sourcePort, portValue); }, type);
					menu.AddItem(new GUIContent("Make Linked Variable", icon, null), false, (o)=>
					{
						var bbVar = graph.blackboard.AddVariable(sourcePort.name, sourcePort.type);
						graph.AddVariableGet((System.Type)o, bbVar.name, pos, sourcePort, portValue);
					}, type);
				}
			}


			//Constructors
			if ( !type.IsAbstract && !type.IsInterface && !type.IsPrimitive && type != typeof(string) ){
				foreach (var _c in type.RTGetConstructors()){
					var c = _c;
					if (!c.IsPublic || c.IsObsolete()){
						continue;
					}

					if (sourcePort is FlowInput || sourcePort is FlowOutput){
						continue;
					}

					var parameters = c.GetParameters();
					if (sourcePort is ValueOutput){
						if (!parameters.Any(p => p.ParameterType.IsAssignableFrom(sourcePort.type))){
							continue;
						}
					}

					if (sourcePort is ValueInput){
						if (!sourcePort.type.IsAssignableFrom(type)){
							continue;
						}
					}

					var categoryName = baseCategory + type.FriendlyName() + "/" + "Constructors/";
					var name = categoryName + c.SignatureName();

					if (typeof(Component).IsAssignableFrom(type)){
						if (type == typeof(Transform)){
							continue;
						}
						var stubType = typeof(NewComponent<>).MakeGenericType(type);
						menu.AddItem( new GUIContent(name, icon, null), false, ()=>{ graph.AddSimplexNode(stubType, pos, sourcePort, dropInstance); } );
						continue;
					}

					if (typeof(ScriptableObject).IsAssignableFrom(type)){
						var stubType = typeof(NewScriptableObject<>).MakeGenericType(type);
						menu.AddItem( new GUIContent(name, icon, null), false, ()=>{ graph.AddSimplexNode(stubType, pos, sourcePort, dropInstance); } );
						continue;
					}

					if (c.GetParameters().Length > 0){
						menu.AddItem( new GUIContent(name, icon, null), false, (o)=>{ graph.AddContructorNode( (ConstructorInfo)o, pos, sourcePort, dropInstance ); }, c);
					}
				}
			}

			//Methods
			var methods = type.RTGetMethods().ToList();
			methods.AddRange( type.GetExtensionMethods() );
			foreach (var _m in methods.OrderBy(_m => !_m.IsStatic).OrderBy(_m => _m.IsSpecialName).OrderBy(_m => _m.DeclaringType != type) ){
				var m = _m;
				if (!m.IsPublic || m.IsObsolete()){
					continue;
				}

				var parameters = m.GetParameters();
				if (sourcePort is ValueOutput){
					if (type != sourcePort.type){
						if (!parameters.Any(p => p.ParameterType.IsAssignableFrom(sourcePort.type))){
							continue;
						}
					}
				}

				if (sourcePort is ValueInput){
					if (!sourcePort.type.IsAssignableFrom(m.ReturnType) && !parameters.Any(p => p.IsOut && sourcePort.type.IsAssignableFrom(p.ParameterType) ) ){
						continue;
					}
				}

				if (sourcePort is FlowInput || sourcePort is FlowOutput){
					if (m.ReturnType != typeof(void)){
						continue;
					}
				}

				var categoryName = baseCategory + type.FriendlyName() + "/" + (m.IsPropertyAccessor() ? "Properties/" : "Methods/");
				var isExtension = m.IsExtensionMethod();
				var isGeneric = m.IsGenericMethod && m.GetGenericArguments().Length == 1;
				if (m.DeclaringType != type){ categoryName += isExtension? "Extensions/" : "Inherited/"; }
				if (isGeneric){
					var arg1 = m.GetGenericArguments()[0];
					var constrains = arg1.GetGenericParameterConstraints();
					var constrainType = constrains.Length == 0? typeof(object) : constrains[0];
					foreach(var t in preferedTypesList){
						if (constrainType.IsAssignableFrom(t)){
							var genericMethod = m.MakeGenericMethod(t);
							var genericCategory = categoryName + m.SignatureName() + "/";
							var genericName = genericCategory + genericMethod.SignatureName();
							menu.AddItem(new GUIContent(genericName, icon, null), false, (o)=>{ graph.AddMethodNode((MethodInfo)o, pos, sourcePort, dropInstance); }, genericMethod);
						}
					}
					continue;
				}
				var name = categoryName + m.SignatureName();
				menu.AddItem(new GUIContent(name, icon, null), false, (o)=>{ graph.AddMethodNode((MethodInfo)o, pos, sourcePort, dropInstance); }, m);
			}

			//Fields
			foreach (var _f in type.RTGetFields()){
				var f = _f;
				if (!f.IsPublic || f.IsObsolete()){
					continue;
				}

				var isReadOnly = f.IsReadOnly();
				var isConstant = f.IsConstant();

				if (sourcePort is ValueOutput){
					if (type != sourcePort.type){
						if (isReadOnly || !f.FieldType.IsAssignableFrom(sourcePort.type)){
							continue;
						}
					}
				}

				if (sourcePort is ValueInput){
					if (!sourcePort.type.IsAssignableFrom(f.FieldType)){
						continue;
					}
				}

				//UnityEvent
				if (typeof(UnityEventBase).IsAssignableFrom(f.FieldType) ){
					var eventCategoryName = baseCategory + type.FriendlyName() + "/Events/";
					menu.AddItem(new GUIContent(eventCategoryName + f.Name, icon, null), false, ()=>{ AddUnityEventNode(graph, f, pos, sourcePort, dropInstance); });
				}


				var categoryName = baseCategory + type.FriendlyName() + "/Fields/";
				if (f.DeclaringType != type){ categoryName += "More/"; }

				var nameForGet = categoryName + (isConstant? "constant " + f.Name : "Get " + f.Name);
				menu.AddItem(new GUIContent(nameForGet, icon, null), false, (o)=>{ graph.AddFieldGetNode((FieldInfo)o, pos, sourcePort, dropInstance); }, f);

				if (!isReadOnly){
					var nameForSet = categoryName + "Set " + f.Name;
					menu.AddItem(new GUIContent(nameForSet, icon, null), false, (o)=>{ graph.AddFieldSetNode((FieldInfo)o, pos, sourcePort, dropInstance); }, f);
				}
			}

/*
			//c# Events
			foreach(var _e in type.RTGetEvents()){
				var e = _e;
				if (e.IsObsolete()){
					continue;
				}

				var eventCategoryName = baseCategory + type.FriendlyName() + "/Events/";
				menu.AddItem(new GUIContent(eventCategoryName + e.Name), icon, null), false, ()=>{ AddSharpEventNode(graph, e, pos, sourcePort, dropInstance); });
			}
*/


			return menu;
		}


		///Simplex Nodes
		public static UnityEditor.GenericMenu AppendSimplexNodesMenu(this FlowGraph graph, UnityEditor.GenericMenu menu, string baseCategory, Vector2 pos, Port sourcePort, object dropInstance){
			foreach (var _info in EditorUtils.GetScriptInfosOfType(typeof(SimplexNode))){
				var info = _info;

				if (sourcePort != null){
					var outProperties = info.type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
					var method = info.type.GetMethod("Invoke");
					if (method != null){
						if (sourcePort is ValueOutput){
							var parameterTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
							if (parameterTypes.Length == 0 || !parameterTypes.Any(t => t.IsAssignableFrom(sourcePort.type))){
								continue;
							}						
						}
						if (sourcePort is ValueInput){
							if ( !sourcePort.type.IsAssignableFrom(method.ReturnType) && !outProperties.Any(p => sourcePort.type.IsAssignableFrom(p.PropertyType) ) ){
								continue;
							}
						}
						if (sourcePort is FlowOutput || sourcePort is FlowInput){
							if ( method.ReturnType != typeof(void) && method.ReturnType != typeof(System.Collections.IEnumerator) ){
								continue;
							}
							if (info.type.IsSubclassOf(typeof(ExtractorNode))){
								continue;
							}
						}
					}
				}

				var category = string.Join("/", new string[]{ baseCategory, info.category, info.name }).TrimStart('/');
				menu.AddItem(new GUIContent(category, info.icon, info.description), false, ()=>{ graph.AddSimplexNode(info.type, pos, sourcePort, dropInstance); } );
			}
			return menu;
		}


		///Variable based nodes
		public static UnityEditor.GenericMenu AppedVariableNodesMenu(this FlowGraph graph, UnityEditor.GenericMenu menu, string baseCategory, Vector2 pos, Port sourcePort, object dropInstance){
			if (!string.IsNullOrEmpty(baseCategory)){
				baseCategory += "/";
			}

			if (sourcePort != null){
				if (sourcePort is ValueInput){
					var category = baseCategory + "Blackboard/";
					
					var name = string.Format("Get Of Type ({0})", sourcePort.type.FriendlyName());
					var genericType = typeof(GetVariable<>).MakeGenericType(new System.Type[]{sourcePort.type});
					menu.AddItem(new GUIContent(category + name), false, ()=>{ graph.AddFlowNode(genericType, pos, sourcePort, dropInstance); });

					var name2 = string.Format("Get Other Of Type ({0})", sourcePort.type.FriendlyName());
					var genericType2 = typeof(GetOtherVariable<>).MakeGenericType(new System.Type[]{sourcePort.type});
					menu.AddItem(new GUIContent(category + name2), false, ()=>{ graph.AddFlowNode(genericType2, pos, sourcePort, dropInstance); });
				}
				if (sourcePort is ValueOutput){
					var category = baseCategory + "Blackboard/";
					
					var name = string.Format("Set Of Type ({0})", sourcePort.type.FriendlyName());
					var genericType = typeof(SetVariable<>).MakeGenericType(new System.Type[]{sourcePort.type});
					menu.AddItem(new GUIContent(category + name), false, ()=>{ graph.AddFlowNode(genericType, pos, sourcePort, dropInstance); });

					var name2 = string.Format("Set Other Of Type ({0})", sourcePort.type.FriendlyName());
					var genericType2 = typeof(SetOtherVariable<>).MakeGenericType(new System.Type[]{sourcePort.type});
					menu.AddItem(new GUIContent(category + name2), false, ()=>{ graph.AddFlowNode(genericType2, pos, sourcePort, dropInstance); });
				}
			}

			var variables = new Dictionary<IBlackboard, List<Variable>>();
			if (graph.blackboard != null){
				variables[graph.blackboard] = graph.blackboard.variables.Values.ToList();
			}
			foreach(var globalBB in GlobalBlackboard.allGlobals){
				variables[globalBB] = globalBB.variables.Values.ToList();
			}

			foreach (var pair in variables){
				foreach(var _bbVar in pair.Value){
					var bbVar = _bbVar;
					var category = baseCategory + "Blackboard/";
					var finalName = pair.Key == graph.blackboard? bbVar.name : string.Format("{0}/{1}", pair.Key.name, bbVar.name);

					if ( sourcePort == null || ( sourcePort is ValueInput && sourcePort.type.IsAssignableFrom(bbVar.varType) ) ){
						menu.AddItem(new GUIContent(category + "Get " + finalName, null, "Get Variable"), false, ()=>{ graph.AddVariableGet(bbVar.varType, finalName, pos, sourcePort, dropInstance); });
					}
					if ( sourcePort == null || sourcePort is FlowOutput || (sourcePort is ValueOutput && bbVar.varType.IsAssignableFrom(sourcePort.type) )  ){
						menu.AddItem(new GUIContent(category + "Set " + finalName, null, "Set Variable"), false, ()=>{ graph.AddVariableSet(bbVar.varType, finalName, pos, sourcePort, dropInstance); });
					}
				}
			}
			return menu;
		}

		///Macro Nodes
		public static UnityEditor.GenericMenu AppendMacroNodesMenu(this FlowGraph graph, UnityEditor.GenericMenu menu, string baseCategory, Vector2 pos, Port sourcePort, object dropInstance){
			var projectMacroGUIDS = UnityEditor.AssetDatabase.FindAssets("t:Macro");
			foreach(var guid in projectMacroGUIDS){
				var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
				var macro = (Macro)UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(Macro));
				if (macro != graph){
					
					if (sourcePort is ValueOutput || sourcePort is FlowOutput){
						if ( !macro.inputDefinitions.Select(d => d.type).Any(d => d.IsAssignableFrom(sourcePort.type)) ){
							continue;
						}
					}

					if (sourcePort is ValueInput || sourcePort is FlowInput){
						if ( !macro.outputDefinitions.Select(d => d.type).Any(d => sourcePort.type.IsAssignableFrom(d)) ){
							continue;
						}
					}

					var category = string.Join("/", new string[]{ baseCategory, macro.name }).TrimStart('/');
					menu.AddItem(new GUIContent(category, null, macro.graphComments), false, ()=>{ graph.AddMacroNode(macro, pos, sourcePort, dropInstance); });
				}
			}

			if (sourcePort == null){
				menu.AddItem(new GUIContent("MACROS/Create New...", null, "Create a new macro"), false, ()=>
				{
					var newMacro = EditorUtils.CreateAsset<Macro>(true);
					if (newMacro != null){
						var wrapper = graph.AddNode<MacroNodeWrapper>(pos);
						wrapper.macro = newMacro;
					}
				});
			}
			return menu;
		}

	}
}

#endif