using System;

namespace ParadoxNotion.Design{

    public class AllowDerivedAttribute : Attribute
    {
    }

    public class GenericFieldAttribute : Attribute
    {
    }

    public class ReorderableListAttribute : Attribute
    {
    }

    ///Marker attribute to include class or method in the aot spoof generation
    public class SpoofAOTAttribute : Attribute{}

	///To exclude a class from being listed. Abstract classes are not listed anyway.
	[AttributeUsage(AttributeTargets.Class)]
	public class DoNotListAttribute : Attribute{}

	///Use for friendly names
	public class NameAttribute : Attribute{
		public string name;
		public NameAttribute(string name){
			this.name = name;
		}
	}

	///Use to give a description
	public class DescriptionAttribute : Attribute{
		public string description;
		public DescriptionAttribute(string description){
			this.description = description;
		}
	}

	///Use for categorization
	[AttributeUsage(AttributeTargets.Class)]
	public class CategoryAttribute : Attribute{
		public string category;
		public int priority;
		public CategoryAttribute(string category, int priority = 0){
			this.category = category;
			this.priority = priority;
		}
	}

	///When a class is associated with an icon
	[AttributeUsage(AttributeTargets.Class)]
	public class IconAttribute : Attribute{
		public string iconName;
		public bool fixedColor;
		public string runtimeIconTypeCallback;
		public IconAttribute(string iconName = "", bool fixedColor = false, string runtimeIconTypeCallback = ""){
			this.iconName = iconName;
			this.fixedColor = fixedColor;
			this.runtimeIconTypeCallback = runtimeIconTypeCallback;
		}
	}	

	///When a class is associated with a color (provide in hex string without "#")
	[AttributeUsage(AttributeTargets.Class)]
	public class ColorAttribute : Attribute{
		public string hexColor;
		public ColorAttribute(string hexColor){
			this.hexColor = hexColor;
		}
	}	

	///When a class should for some reason be marked as protected
	[AttributeUsage(AttributeTargets.Class)]
	public class ProtectedAttribute : Attribute{}


	///Makes the int field show as layerfield
	[AttributeUsage(AttributeTargets.Field)]
	public class LayerFieldAttribute : Attribute{}

	///Makes the string field show as tagfield
	[AttributeUsage(AttributeTargets.Field)]
	public class TagFieldAttribute : Attribute{}

	///Makes the string field show as text field with specified height
	[AttributeUsage(AttributeTargets.Field)]
	public class TextAreaFieldAttribute : Attribute{
		public float height;
		public TextAreaFieldAttribute(float height){
			this.height = height;
		}
	}

	///Use on top of any type of field to restict values to the provided ones through a popup by either providing a params array for Valuetypes,
	///or a static property of a class in the form of "MyClass.MyProperty"
	[AttributeUsage(AttributeTargets.Field)]
	public class PopupFieldAttribute : Attribute{
		public object[] values;
		public string staticPath;
		public PopupFieldAttribute(params object[] values){
			this.values = values;
		}
		public PopupFieldAttribute(string staticPath){
			this.staticPath = staticPath;
		}
	}

	///Use on to of any field to show it only if the provided field is equal to the provided check value
	[AttributeUsage(AttributeTargets.Field)]
	public class ShowIfAttribute : Attribute{
		public string fieldName;
		public int checkValue;
		public ShowIfAttribute(string fieldName, int checkValue){
			this.fieldName = fieldName;
			this.checkValue = checkValue;
		}
	}

    ///Makes the float or integer field show as slider
	[AttributeUsage(AttributeTargets.Field)]
	public class SliderFieldAttribute : Attribute{
		public float left;
		public float right;
		public SliderFieldAttribute(float left, float right){
			this.left = left;
			this.right = right;
		}
		public SliderFieldAttribute(int left, int right){
			this.left = left;
			this.right = right;
		}
	}

	///Forces the field to show as a Unity Object field. Usefull for interface fields
	[AttributeUsage(AttributeTargets.Field)]
	public class ForceObjectFieldAttribute : Attribute{}

	///Helper attribute. Designates that the field is required not to be null or string.empty
	[AttributeUsage(AttributeTargets.Field)]
	public class RequiredFieldAttribute : Attribute{}
}