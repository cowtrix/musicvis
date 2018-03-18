using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion;
using System.Linq.Expressions;

namespace FlowCanvas
{

    ///Responsible for internal -connection level- value conversions
    public static class TypeConverter {

		[ParadoxNotion.Design.SpoofAOT]
		public static ValueHandler<T> GetConverterFuncFromTo<T>(Type sourceType, Type targetType, ValueHandler<object> func){

			//Flow only connect to Flow
			if (sourceType == typeof(Flow) && sourceType != targetType){
				return null;
			}

			//Wilds
			if (targetType == typeof(Wild)){
				return ()=>{ return default(T); };
			}

			//assignables or upcasting
			if (targetType.RTIsAssignableFrom(sourceType) || targetType.RTIsSubclassOf(sourceType)){
				return ()=>{ return (T)func(); };
			}

			//convertibles
			if (typeof(IConvertible).RTIsAssignableFrom(targetType) && typeof(IConvertible).RTIsAssignableFrom(sourceType)){
				return ()=> { return (T)Convert.ChangeType(func(), targetType); };
			}

			//handles implicit/explicit and prety much everything else.
			//invoke done with reflection to support all platforms. 
			UnaryExpression exp = null;
			if (ReflectionTools.CanConvert(sourceType, targetType, out exp)){
				return ()=>{ return (T)exp.Method.Invoke(null, new object[]{func()} ); };

			}

			///CUSTOM CONVENIENCE CONVERSIONS

			//from anything to string
			if (targetType == typeof(string)){
				return ()=> { try {return (T)(object)(func().ToString());} catch {return default(T);} };
			}

			//from convertible to Vector3
			if (targetType == typeof(Vector3) && typeof(IConvertible).RTIsAssignableFrom(sourceType)){
				return ()=>
				{
					var f = (float)Convert.ChangeType(func(), typeof(float));
					return (T)(object)new Vector3(f, f, f);
				};
			}

			//from component to Vector3 (position)
			if (targetType == typeof(Vector3) && typeof(Component).RTIsAssignableFrom(sourceType) ){
				return () => { try {return (T)(object)((func() as Component).transform.position);} catch {return default(T);} };
			}

			//from gameobject to Vector3 (position)
			if (targetType == typeof(Vector3) && sourceType == typeof(GameObject) ){
				return () => { try {return (T)(object)((func() as GameObject).transform.position);} catch {return default(T);} };
			}

			//from component to component
			if (typeof(Component).RTIsAssignableFrom(targetType) && typeof(Component).RTIsAssignableFrom(sourceType) ){
				return () => { try {return (T)(object)((func() as Component).GetComponent(targetType));} catch {return default(T);} };
			}
				
			//from gameobject to component
			if (typeof(Component).RTIsAssignableFrom(targetType) && sourceType == typeof(GameObject) ){
				return () => { try {return (T)(object)((func() as GameObject).GetComponent(targetType));} catch {return default(T);} };
			}

			//from component to gameobject
			if (targetType == typeof(GameObject) && typeof(Component).RTIsAssignableFrom(sourceType) ){
				return ()=> { try {return (T)(object)((func() as Component).gameObject);} catch {return default(T);} };
			}


			//From IEnumerable to IEnumerable for Lists and Arrays
			if ( typeof(IEnumerable).RTIsAssignableFrom(sourceType) && typeof(IEnumerable).RTIsAssignableFrom(targetType) ){
				try
				{
					var elementFrom = sourceType.IsArray? sourceType.GetElementType() : sourceType.GetGenericArguments().Single();
					var elementTo = targetType.IsArray? targetType.GetElementType() : targetType.GetGenericArguments().Single();
					if (elementTo.RTIsAssignableFrom(elementFrom)){
						return ()=>
						{
							var original = func() as IEnumerable;
							var listType = typeof(List<>).RTMakeGenericType(elementTo);
							var list = (IList)System.Activator.CreateInstance(listType);
							foreach(var o in original){ list.Add(o); }
							return (T)list;
						};
					}
				}
				catch { return null; }
			}

			return null;
		}

		///Is there a convertion available from source type and to target type?
		//This is done only in editor.
		public static bool HasConvertion(Type sourceType, Type targetType){
			return GetConverterFuncFromTo<object>(sourceType, targetType, null) != null;
		}
	}
}