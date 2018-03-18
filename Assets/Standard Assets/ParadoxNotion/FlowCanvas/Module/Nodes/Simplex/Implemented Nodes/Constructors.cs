using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes{

	[Category("Utilities/Constructors")]
	[Description("If 'Try Get Existing' is true, then if there is an existing component of that type already attached to the gameobject, it will be returned instead of adding another instance.")]
	public class NewComponent<T> : CallableFunctionNode<T, GameObject, bool> where T:Component{
		public override T Invoke(GameObject gameObject, bool tryGetExisting){
			T component = null;
			if (gameObject != null){
				if (tryGetExisting){
					component = gameObject.GetComponent<T>();
				}
				if (component == null){
					component = gameObject.AddComponent<T>();
				}
			}
			return component;
		}
	}

	[Category("Utilities/Constructors")]
	public class NewGameObject : CallableFunctionNode<GameObject, string, Vector3, Quaternion>{
		public override GameObject Invoke(string name, Vector3 position, Quaternion rotation){
			var go = new GameObject(name);
			go.transform.position = position;
			go.transform.rotation = rotation;
			return go;
		}
	}

	[Category("Utilities/Constructors")]
	public class NewScriptableObject<T> : CallableFunctionNode<T> where T:ScriptableObject{
		public override T Invoke(){
			return ScriptableObject.CreateInstance<T>();
		}
	}

	[Category("Utilities/Constructors")]
	public class NewVector2 : PureFunctionNode<Vector2, float, float>{
		public override Vector2 Invoke(float x, float y){
			return new Vector2(x, y);
		}
	}

	[Category("Utilities/Constructors")]
	public class NewVector3 : PureFunctionNode<Vector3, float, float, float>{
		public override Vector3 Invoke(float x, float y, float z){
			return new Vector3(x, y, z);
		}
	}

	[Category("Utilities/Constructors")]
	public class NewVector4 : PureFunctionNode<Vector4, float, float, float, float>{
		public override Vector4 Invoke(float x, float y, float z, float w){
			return new Vector4(x, y, z, w);
		}
	}

	[Category("Utilities/Constructors")]
	public class NewQuaternion : PureFunctionNode<Quaternion, float, float, float, float>{
		public override Quaternion Invoke(float x, float y, float z, float w){
			return new Quaternion(x, y, z, w);
		}
	}

	[Category("Utilities/Constructors")]
	public class NewColor : PureFunctionNode<Color, float, float, float, float>{
		public override Color Invoke(float r, float g, float b, float a = 1){
			return new Color(r,g,b,a);
		}
	}

	[Category("Utilities/Constructors")]
	public class NewBounds : PureFunctionNode<Bounds, Vector3, Vector3>{
		public override Bounds Invoke(Vector3 center, Vector3 size){
			return new Bounds(center, size);
		}
	}

	[Category("Utilities/Constructors")]
	public class NewRect : PureFunctionNode<Rect, float, float, float, float>{
		public override Rect Invoke(float left, float top, float width, float height){
			return new Rect(left, top, width, height);
		}
	}

	[Category("Utilities/Constructors")]
	public class NewRay : PureFunctionNode<Ray, Vector3, Vector3>{
		public override Ray Invoke(Vector3 origin, Vector3 direction){
			return new Ray(origin, direction);
		}
	}
}