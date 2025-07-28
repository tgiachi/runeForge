using System.Reflection;
using Runeforge.Engine.Attributes.Scripts;
using Runeforge.Engine.Data.Internal.Scripts;

namespace Runeforge.Engine.Utils;

/// <summary>
/// Static class for scanning classes and generating script function descriptors from attributes
/// </summary>
public static class ScriptDescriptorScanner
{
    /// <summary>
    /// Scans a single class for ScriptModule and ScriptFunction attributes and generates descriptors
    /// </summary>
    /// <param name="classType">The class type to scan</param>
    /// <returns>List of ScriptFunctionDescriptor for the class, empty if no ScriptModule attribute found</returns>
    public static List<ScriptFunctionDescriptor> ScanClass(Type classType)
    {
        ArgumentNullException.ThrowIfNull(classType);

        var descriptors = new List<ScriptFunctionDescriptor>();

        // Check if class has ScriptModule attribute
        var moduleAttribute = classType.GetCustomAttribute<ScriptModuleAttribute>();
        if (moduleAttribute == null)
        {
            return descriptors; // Return empty list if no ScriptModule attribute
        }

        var moduleName = moduleAttribute.Name;

        // Get all public methods with ScriptFunction attribute
        var methods = classType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
            .Where(m => m.GetCustomAttribute<ScriptFunctionAttribute>() != null);

        foreach (var method in methods)
        {
            var functionAttribute = method.GetCustomAttribute<ScriptFunctionAttribute>()!;
            var descriptor = CreateFunctionDescriptor(moduleName, method, functionAttribute);
            descriptors.Add(descriptor);
        }

        return descriptors;
    }

    /// <summary>
    /// Scans multiple classes for ScriptModule and ScriptFunction attributes
    /// </summary>
    /// <param name="classTypes">Array of class types to scan</param>
    /// <returns>Combined list of ScriptFunctionDescriptor from all classes</returns>
    public static List<ScriptFunctionDescriptor> ScanClasses(params Type[] classTypes)
    {
        ArgumentNullException.ThrowIfNull(classTypes);

        var allDescriptors = new List<ScriptFunctionDescriptor>();

        foreach (var classType in classTypes)
        {
            var descriptors = ScanClass(classType);
            allDescriptors.AddRange(descriptors);
        }

        return allDescriptors;
    }

    /// <summary>
    /// Scans all classes in an assembly for ScriptModule and ScriptFunction attributes
    /// </summary>
    /// <param name="assembly">Assembly to scan</param>
    /// <returns>List of ScriptFunctionDescriptor from all applicable classes in the assembly</returns>
    public static List<ScriptFunctionDescriptor> ScanAssembly(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        var allDescriptors = new List<ScriptFunctionDescriptor>();

        // Get all types with ScriptModule attribute
        var scriptModuleTypes = assembly.GetTypes()
            .Where(t => t.GetCustomAttribute<ScriptModuleAttribute>() != null);

        foreach (var type in scriptModuleTypes)
        {
            var descriptors = ScanClass(type);
            allDescriptors.AddRange(descriptors);
        }

        return allDescriptors;
    }

    /// <summary>
    /// Creates a ScriptFunctionDescriptor from a method and its ScriptFunction attribute
    /// </summary>
    /// <param name="moduleName">Name of the module containing the function</param>
    /// <param name="method">MethodInfo to analyze</param>
    /// <param name="functionAttribute">ScriptFunction attribute from the method</param>
    /// <returns>ScriptFunctionDescriptor for the method</returns>
    private static ScriptFunctionDescriptor CreateFunctionDescriptor(
        string moduleName,
        MethodInfo method,
        ScriptFunctionAttribute functionAttribute)
    {
        var descriptor = new ScriptFunctionDescriptor
        {
            ModuleName = moduleName,
            FunctionName = method.Name,
            Help = functionAttribute.HelpText,
            ReturnType = GetFriendlyTypeName(method.ReturnType),
            RawReturnType = method.ReturnType
        };

        // Process parameters
        var parameters = method.GetParameters();
        foreach (var param in parameters)
        {
            var paramType = param.ParameterType;
            var paramName = param.Name ?? "param";

            // Handle params arrays - add variadic indicator
            if (param.IsDefined(typeof(ParamArrayAttribute), false))
            {
                paramName = "..." + paramName; // Indicate it's variadic in EmmyLua style
            }

            var paramDescriptor = new ScriptFunctionParameterDescriptor(
                ParameterName: paramName,
                ParameterType: GetFriendlyTypeName(paramType),
                RawParameterType: paramType,
                ParameterTypeString: paramType.ToString()
            );

            descriptor.Parameters.Add(paramDescriptor);
        }

        return descriptor;
    }

    /// <summary>
    /// Gets a friendly type name for display purposes
    /// </summary>
    /// <param name="type">Type to get friendly name for</param>
    /// <returns>Friendly type name string</returns>
    private static string GetFriendlyTypeName(Type type)
    {
        if (type == typeof(void))
            return "void";

        if (type == typeof(string))
            return "string";

        if (type == typeof(int))
            return "int";

        if (type == typeof(long))
            return "long";

        if (type == typeof(float))
            return "float";

        if (type == typeof(double))
            return "double";

        if (type == typeof(bool))
            return "bool";

        if (type == typeof(object))
            return "object";

        // Handle nullable types
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            var underlyingType = Nullable.GetUnderlyingType(type);
            return GetFriendlyTypeName(underlyingType!) + "?";
        }

        // Handle arrays
        if (type.IsArray)
        {
            var elementType = type.GetElementType()!;
            return GetFriendlyTypeName(elementType) + "[]";
        }

        // Handle generic types
        if (type.IsGenericType)
        {
            var genericTypeName = type.Name.Split('`')[0];
            var genericArgs = type.GetGenericArguments();
            var genericArgNames = string.Join(", ", genericArgs.Select(GetFriendlyTypeName));
            return $"{genericTypeName}<{genericArgNames}>";
        }

        // For enums and other types, return the simple name
        return type.Name;
    }

    /// <summary>
    /// Scans classes and extracts unique enum types referenced in method signatures
    /// </summary>
    /// <param name="classTypes">Array of class types to scan for enums</param>
    /// <returns>List of unique enum types found</returns>
    public static List<Type> ExtractEnumTypes(params Type[] classTypes)
    {
        ArgumentNullException.ThrowIfNull(classTypes);

        var enumTypes = new HashSet<Type>();

        foreach (var classType in classTypes)
        {
            // Check if class has ScriptModule attribute
            if (classType.GetCustomAttribute<ScriptModuleAttribute>() == null)
                continue;

            // Get all public methods with ScriptFunction attribute
            var methods = classType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                .Where(m => m.GetCustomAttribute<ScriptFunctionAttribute>() != null);

            foreach (var method in methods)
            {
                // Extract enums from return type
                ExtractEnumsFromType(method.ReturnType, enumTypes);

                // Extract enums from parameter types
                foreach (var param in method.GetParameters())
                {
                    ExtractEnumsFromType(param.ParameterType, enumTypes);
                }
            }
        }

        return enumTypes.ToList();
    }

    /// <summary>
    /// Recursively extracts all enum types from a given type (including nested generics, arrays, delegates, etc.)
    /// </summary>
    /// <param name="type">Type to analyze</param>
    /// <param name="enumTypes">HashSet to collect found enum types</param>
    private static void ExtractEnumsFromType(Type type, HashSet<Type> enumTypes)
    {
        // Direct enum check
        if (type.IsEnum)
        {
            enumTypes.Add(type);
            return;
        }

        // Handle nullable types
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null)
            {
                ExtractEnumsFromType(underlyingType, enumTypes);
            }
            return;
        }

        // Handle arrays
        if (type.IsArray)
        {
            var elementType = type.GetElementType();
            if (elementType != null)
            {
                ExtractEnumsFromType(elementType, enumTypes);
            }
            return;
        }

        // Handle generic types (List<T>, Dictionary<K,V>, etc.)
        if (type.IsGenericType)
        {
            foreach (var genericArg in type.GetGenericArguments())
            {
                ExtractEnumsFromType(genericArg, enumTypes);
            }
        }

        // Handle delegates (Action, Func, custom delegates)
        if (typeof(Delegate).IsAssignableFrom(type) || type.BaseType == typeof(MulticastDelegate))
        {
            var invokeMethod = type.GetMethod("Invoke");
            if (invokeMethod != null)
            {
                // Extract from return type
                ExtractEnumsFromType(invokeMethod.ReturnType, enumTypes);

                // Extract from parameters
                foreach (var param in invokeMethod.GetParameters())
                {
                    ExtractEnumsFromType(param.ParameterType, enumTypes);
                }
            }
        }

        // Handle custom classes/structs - check their public properties and fields
        if (type.IsClass && type != typeof(string) && type != typeof(object) && !type.IsPrimitive)
        {
            ExtractEnumsFromCustomType(type, enumTypes);
        }
    }

    /// <summary>
    /// Extracts enum types from custom classes by analyzing their properties and fields
    /// </summary>
    /// <param name="type">Custom type to analyze</param>
    /// <param name="enumTypes">HashSet to collect found enum types</param>
    /// <param name="visitedTypes">HashSet to prevent infinite recursion</param>
    private static void ExtractEnumsFromCustomType(Type type, HashSet<Type> enumTypes, HashSet<Type>? visitedTypes = null)
    {
        visitedTypes ??= new HashSet<Type>();

        // Prevent infinite recursion
        if (visitedTypes.Contains(type))
            return;

        visitedTypes.Add(type);

        // Analyze public properties
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var property in properties)
        {
            ExtractEnumsFromType(property.PropertyType, enumTypes);
        }

        // Analyze public fields
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (var field in fields)
        {
            ExtractEnumsFromType(field.FieldType, enumTypes);
        }
    }

    /// <summary>
    /// Extracts all custom types used in method signatures recursively
    /// </summary>
    /// <param name="classTypes">Array of class types to scan</param>
    /// <returns>List of all custom types found</returns>
    public static List<Type> ExtractAllCustomTypes(params Type[] classTypes)
    {
        ArgumentNullException.ThrowIfNull(classTypes);

        var customTypes = new HashSet<Type>();

        foreach (var classType in classTypes)
        {
            // Check if class has ScriptModule attribute
            if (classType.GetCustomAttribute<ScriptModuleAttribute>() == null)
                continue;

            // Get all public methods with ScriptFunction attribute
            var methods = classType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                .Where(m => m.GetCustomAttribute<ScriptFunctionAttribute>() != null);

            foreach (var method in methods)
            {
                // Extract types from return type
                ExtractCustomTypesFromType(method.ReturnType, customTypes);

                // Extract types from parameter types
                foreach (var param in method.GetParameters())
                {
                    ExtractCustomTypesFromType(param.ParameterType, customTypes);
                }
            }
        }

        return customTypes.ToList();
    }

    /// <summary>
    /// Recursively extracts all custom (non-primitive) types from a given type
    /// </summary>
    /// <param name="type">Type to analyze</param>
    /// <param name="customTypes">HashSet to collect found custom types</param>
    /// <param name="visitedTypes">HashSet to prevent infinite recursion</param>
    private static void ExtractCustomTypesFromType(Type type, HashSet<Type> customTypes, HashSet<Type>? visitedTypes = null)
    {
        visitedTypes ??= new HashSet<Type>();

        // Prevent infinite recursion
        if (visitedTypes.Contains(type))
            return;

        visitedTypes.Add(type);

        // Skip primitive types, string, and basic types
        if (type.IsPrimitive || type == typeof(string) || type == typeof(object) || type == typeof(void))
            return;

        // Add custom types (classes, structs, enums)
        if (type.IsClass || type.IsValueType || type.IsEnum)
        {
            customTypes.Add(type);
        }

        // Handle nullable types
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null)
            {
                ExtractCustomTypesFromType(underlyingType, customTypes, visitedTypes);
            }
            return;
        }

        // Handle arrays
        if (type.IsArray)
        {
            var elementType = type.GetElementType();
            if (elementType != null)
            {
                ExtractCustomTypesFromType(elementType, customTypes, visitedTypes);
            }
            return;
        }

        // Handle generic types
        if (type.IsGenericType)
        {
            foreach (var genericArg in type.GetGenericArguments())
            {
                ExtractCustomTypesFromType(genericArg, customTypes, visitedTypes);
            }
        }

        // Handle delegates
        if (typeof(Delegate).IsAssignableFrom(type) || type.BaseType == typeof(MulticastDelegate))
        {
            var invokeMethod = type.GetMethod("Invoke");
            if (invokeMethod != null)
            {
                // Extract from return type
                ExtractCustomTypesFromType(invokeMethod.ReturnType, customTypes, visitedTypes);

                // Extract from parameters
                foreach (var param in invokeMethod.GetParameters())
                {
                    ExtractCustomTypesFromType(param.ParameterType, customTypes, visitedTypes);
                }
            }
        }

        // For custom classes/structs, analyze their members
        if (type.IsClass && type != typeof(string) && !type.IsPrimitive)
        {
            AnalyzeCustomTypeMembers(type, customTypes, visitedTypes);
        }
    }

    /// <summary>
    /// Analyzes members of a custom type to find more custom types
    /// </summary>
    /// <param name="type">Type to analyze</param>
    /// <param name="customTypes">HashSet to collect found custom types</param>
    /// <param name="visitedTypes">HashSet to prevent infinite recursion</param>
    private static void AnalyzeCustomTypeMembers(Type type, HashSet<Type> customTypes, HashSet<Type> visitedTypes)
    {
        // Analyze public properties
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var property in properties)
        {
            ExtractCustomTypesFromType(property.PropertyType, customTypes, visitedTypes);
        }

        // Analyze public fields
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (var field in fields)
        {
            ExtractCustomTypesFromType(field.FieldType, customTypes, visitedTypes);
        }
    }

    /// <summary>
    /// Convenience method to scan classes and generate both descriptors and all related types
    /// </summary>
    /// <param name="classTypes">Array of class types to scan</param>
    /// <returns>Tuple containing the descriptors, enum types, and all custom types</returns>
    public static (List<ScriptFunctionDescriptor> Descriptors, List<Type> EnumTypes, List<Type> CustomTypes) ScanClassesWithAllTypes(params Type[] classTypes)
    {
        var descriptors = ScanClasses(classTypes);
        var enumTypes = ExtractEnumTypes(classTypes);
        var customTypes = ExtractAllCustomTypes(classTypes);
        return (descriptors, enumTypes, customTypes);
    }
}
