using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public static class TypeExtensions
{
    public static T GetAttributeOfType<T>(this Enum enumVal) where T : System.Attribute
    {
        var type = enumVal.GetType();
        var memInfo = type.GetMember(enumVal.ToString());
        var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
        return (attributes.Length > 0) ? (T)attributes[0] : null;
    }

    public static bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
    {
        while (toCheck != null && toCheck != typeof(object))
        {
            var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
            if (generic == cur)
            {
                return true;
            }
            toCheck = toCheck.BaseType;
        }
        return false;
    }

    public static bool TypeInheritsFrom(Type testType, Type doesInheritType)
    {
        if (testType == doesInheritType)
        {
            return true;
        }
        var baseType = testType.BaseType;
        if (baseType == null)
        {
            return false;
        }
        return TypeInheritsFrom(baseType, doesInheritType);
    }

    public static List<Type> GetAllTypesImplementingInterface(this Type interfaceType, Type assemblyType = null)
    {
        if (!interfaceType.IsInterface)
        {
            throw new Exception("Must be an interface type!");
        }
        var result = new List<Type>();

        var assembly = Assembly.GetAssembly(assemblyType ?? interfaceType);
        var allTypes = assembly.GetTypes();
        for (var i = 0; i < allTypes.Length; i++)
        {
            if (allTypes[i].GetInterfaces().Contains(interfaceType))
            {
                result.Add(allTypes[i]);
            }
        }

        return result;
    }

    public static T GetAttribute<T>(this Type t) where T : Attribute
    {
        return (T)t.GetCustomAttributes(typeof(T), true).FirstOrDefault();
    }

    public static List<Type> GetAllTypesWithAttribute<T>() where T : Attribute
    {
        var result = new List<Type>();
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            var allTypes = assembly.GetTypes();
            foreach (Type type in allTypes)
            {
                var attribute = type.GetCustomAttributes(typeof(T), true).FirstOrDefault();
                if (attribute != null)
                {
                    result.Add(type);
                }
            }
        }
        return result;
    }
}