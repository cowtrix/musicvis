#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using ParadoxNotion;
using UnityObject = UnityEngine.Object;

namespace ParadoxNotion.Design{

	///Have some commonly stuff used across most inspectors and helper functions. Keep outside of Editor folder since many runtime classes use this in #if UNITY_EDITOR
	///This is a partial class. Different implementation provide different tools, so that everything is referenced from within one class.
	[InitializeOnLoad]
	public static partial class EditorUtils{

		readonly public static Texture2D playIcon     = EditorGUIUtility.FindTexture("d_PlayButton");
		readonly public static Texture2D pauseIcon    = EditorGUIUtility.FindTexture("d_PauseButton");
		readonly public static Texture2D stepIcon     = EditorGUIUtility.FindTexture("d_StepButton");
		readonly public static Texture2D viewIcon     = EditorGUIUtility.FindTexture("d_ViewToolOrbit On");
		readonly public static Texture2D csIcon       = EditorGUIUtility.FindTexture("cs Script Icon");
		readonly public static Texture2D jsIcon       = EditorGUIUtility.FindTexture("Js Script Icon");
		readonly public static Texture2D tagIcon      = EditorGUIUtility.FindTexture("d_FilterByLabel");
		readonly public static Texture2D searchIcon   = EditorGUIUtility.FindTexture("Search Icon");
		readonly public static Texture2D warningIcon  = EditorGUIUtility.FindTexture("d_console.warnicon.sml");
		readonly public static Texture2D redCircle    = EditorGUIUtility.FindTexture("d_winbtn_mac_close");
		readonly public static Texture2D folderIcon   = EditorGUIUtility.FindTexture("Folder Icon");
		readonly public static Texture2D favoriteIcon = EditorGUIUtility.FindTexture("Favorite Icon");

		readonly public static Color lightOrange = new Color(1, 0.9f, 0.4f);
		readonly public static Color lightBlue   = new Color(0.8f,0.8f,1);
		readonly public static Color lightRed    = new Color(1,0.5f,0.5f, 0.8f);


		//For gathering script/type meta-information
		public class ScriptInfo{
			public Type type;
			public string name;
			public string category;
			public string description;
			public Texture icon;
			public int priority;
		    public int genericPathIndex;

			public ScriptInfo(Type type, string name, string category, int genericPathIndex, int priority = 0){
				this.type = type;
				this.name = name;
				this.category = category;
				this.priority = priority;
			    this.genericPathIndex = genericPathIndex;
				if (type != null){
					var iconAtt = type.RTGetAttribute<IconAttribute>(true);
					icon = iconAtt != null? UserTypePrefs.GetTypeIcon(iconAtt) : null;
					var descAtt = type.RTGetAttribute<DescriptionAttribute>(true);
					description = descAtt != null? descAtt.description : description;
				}
			}
		}

		///Get a list of ScriptInfos of the baseType excluding: the base type, abstract classes, Obsolete classes and those with the DoNotList attribute, categorized as a list of ScriptInfo
		private static Dictionary<Type, List<ScriptInfo>> cachedInfos = new Dictionary<Type, List<ScriptInfo>>();
		public static List<ScriptInfo> GetScriptInfosOfType(Type baseType){

			List<ScriptInfo> infos;
			if (cachedInfos.TryGetValue(baseType, out infos)){
				return infos;
			}

			infos = new List<ScriptInfo>();
			
			var subTypes = GetAssemblyTypes(baseType);
			if (baseType.IsGenericTypeDefinition){
				subTypes = new List<Type>{ baseType };
			}

			foreach (var subType in subTypes){

				if ( subType.IsDefined(typeof(DoNotListAttribute), true) || subType.IsDefined(typeof(ObsoleteAttribute), true) ){
					continue;
				}
				
				if (subType.IsAbstract){
					continue;
				}

				var isGeneric = subType.IsGenericTypeDefinition && subType.GetGenericArguments().Length == 1;

				var scriptName = subType.FriendlyName().SplitCamelCase();
				var scriptCategory = string.Empty;
				var scriptPriority = 0;

				var nameAttribute = subType.GetCustomAttributes(typeof(NameAttribute), false).FirstOrDefault() as NameAttribute;
				if (nameAttribute != null){
					scriptName = nameAttribute.name;
					if (isGeneric && !scriptName.EndsWith("<T>")){
						scriptName += " (T)";
					}
				}

				var categoryAttribute = subType.GetCustomAttributes(typeof(CategoryAttribute), true).FirstOrDefault() as CategoryAttribute;
				if (categoryAttribute != null){
					scriptCategory = categoryAttribute.category;
					scriptPriority = categoryAttribute.priority;
				}

				//add the generic types based on constrains and prefered types list
				if (isGeneric){

					var arg1 = subType.GetGenericArguments()[0];
					var constrains = arg1.GetGenericParameterConstraints();
					var constrainType = constrains.Length == 0? typeof(object) : constrains[0];
					var types = UserTypePrefs.GetPreferedTypesList( constrainType, true );

				    var genericIndex = string.IsNullOrEmpty(scriptCategory) ? 0 : scriptCategory.Count(c => c == '/') + 1;


                    foreach (var t in types){
						{
							var genericType = subType.MakeGenericType(t);
							var genericCategory = scriptCategory + "/" + scriptName + "/" + t.NamespaceToPath() ;
							var genericName = genericType.FriendlyName();
							// var genericCategory = scriptCategory + "/" + scriptName;
							// var genericName = t.NamespaceToPath() + "/" + genericType.FriendlyName();
                            
							infos.Add( new ScriptInfo(genericType, genericName, genericCategory, genericIndex, scriptPriority) );
						}
						
						{
							//append List<T> types
							var listType = typeof(List<>).MakeGenericType(t);
							if (constrainType.IsAssignableFrom(listType)){
								var listFinalType = subType.MakeGenericType(listType);
								var listCategory = scriptCategory + "/" + scriptName + "/" + UserTypePrefs.LIST_MENU_STRING + t.NamespaceToPath();
								var listName = listFinalType.FriendlyName();
								// var listCategory = scriptCategory + "/" + scriptName;
								// var listName = "(List)/" + listFinalType.FriendlyName();
								infos.Add( new ScriptInfo(listFinalType, listName, listCategory, genericIndex, 1000) );
							}
						}

						{
							//append Dictionary<string, T> types
							var dictType = typeof(Dictionary<,>).MakeGenericType(typeof(string), t);
							if (constrainType.IsAssignableFrom(dictType)){
								var dictFinalType = subType.MakeGenericType(dictType);
								var dictCategory = scriptCategory + "/" + scriptName + "/" + UserTypePrefs.DICT_MENU_STRING + t.NamespaceToPath();
								var dictName = dictFinalType.FriendlyName();
								// var dictCategory = scriptCategory + "/" + scriptName;
								// var dictName = "(Dictionary)/" + dictFinalType.FriendlyName();
								infos.Add( new ScriptInfo(dictFinalType, dictName, dictCategory, genericIndex, 1001) );
							}
						}
					}
				
				} else {

					infos.Add(new ScriptInfo(subType, scriptName, scriptCategory, -1, scriptPriority));
				}
			}

		    var order = infos.OrderBy(s => s.priority);
		    var max = infos.Max(i =>
		    {
		        var cat = i.category;
		        if (string.IsNullOrEmpty(cat)) return 1;
		        return cat.Count(c => c == '/') + 2;
		    });
		    for (var i = 0; i <= max - 1; i++)
		    {
		        order = OrderHelper(order, i);
		    }

            // infos = infos.OrderBy(s => s.category + "/" + s.name).ToList();
            // infos = infos.OrderBy(s => s.name).OrderBy(s => s.category).OrderBy(s => s.priority).ToList();
		    infos = order.ToList();
			return cachedInfos[baseType] = infos;
		}

	    private static IOrderedEnumerable<ScriptInfo> OrderHelper(IOrderedEnumerable<ScriptInfo> order, int splitIdx)
	    {
	        return order.ThenBy(i =>
	        {
	            var split = string.IsNullOrEmpty(i.category) ? new string[0] : i.category.Split('/');
	            if (splitIdx == split.Length) return "_0_" + i.name;
	            if (splitIdx > split.Length) return "";
	            if (i.genericPathIndex >=0 && splitIdx == i.genericPathIndex) return "_0_" + split[splitIdx];
	            return "_1_" + split[splitIdx];
	        });
	    }


        //...
        public static string NamespaceToPath(this Type type){
			if (type == null){ return string.Empty; }
			return string.IsNullOrEmpty(type.Namespace)? "No Namespace" : type.Namespace.Split('.').First();
		}


		//Get all base derived types in the current loaded assemplies, excluding the base type itself
		private static Dictionary<Type, List<Type>> cachedSubTypes = new Dictionary<Type, List<Type>>();
		public static List<Type> GetAssemblyTypes(Type baseType){

			List<Type> subTypes;
			if (cachedSubTypes.TryGetValue(baseType, out subTypes)){
				return subTypes;
			}

			subTypes = new List<Type>();

			foreach (var asm in System.AppDomain.CurrentDomain.GetAssemblies()) {
			    try
			    {
			        foreach (var t in asm.GetExportedTypes().Where(t => t.IsSubclassOf(baseType))) {
			            if ( t.IsVisible && !t.IsDefined(typeof(System.ObsoleteAttribute), true)){
							subTypes.Add(t);
						}
			        }
			    }
			    catch
			    {
			        Debug.Log(asm.FullName + " will be excluded");
			        continue;
			    }
			}				
			
			// subTypes = subTypes.OrderBy(t => t.Namespace + "." + t.FriendlyName()).ToList();
			subTypes = subTypes.OrderBy(t => t.FriendlyName()).OrderBy(t => t.Namespace).ToList();
			return cachedSubTypes[baseType] = subTypes;
		}


		//Gets the first type found by providing just the name of the type. Rarely used (currently for upgrading ScriptControl tasks)
		public static Type GetType(string name, Type fallback){
			foreach (var t in GetAssemblyTypes(typeof(object))){
				if (t.Name == name){
					return t;
				}
			}
			return fallback;
		}

		///Opens the MonoScript of a type if existant
		public static bool OpenScriptOfType(Type type){
			foreach (var path in AssetDatabase.GetAllAssetPaths()){
				if (path.EndsWith(type.Name + ".cs") || path.EndsWith(type.Name + ".js")){
					var script = (MonoScript)AssetDatabase.LoadAssetAtPath(path, typeof(MonoScript));
					if (type == script.GetClass()){
						AssetDatabase.OpenAsset(script);
						return true;
					}
				}
			}

			Debug.Log(string.Format("Can't open script of type '{0}', cause a script with the same name does not exist", type.FriendlyName() ));
			return false;
		}


		//Get all scene names (added in build settings)
		public static List<string> GetSceneNames(){
			var allSceneNames = new List<string>();
			foreach (var scene in EditorBuildSettings.scenes){
				if (scene.enabled){
					var name = scene.path.Substring(scene.path.LastIndexOf("/") + 1);
					name = name.Substring(0,name.Length-6);
					allSceneNames.Add(name);
				}
			}

			return allSceneNames;
		}

	}
}

#endif