/*
Copyright 2017 Brandon Bernard

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

	 http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace System.Reflection.CustomExtensions
{
    /// <summary>
    /// BBernard - 2017
    /// Set of custom extensions to help accessing private elements with Brute Force approach
    /// 
    /// NOTE: This is extremely useful when real work-arounds are needed and we do not own or want to 
    ///         copy/paste class code from other projects/assemblies, etc.
    /// 
    /// More info. on the original code this is based on can be found here:
    /// https://chrizyuen.wordpress.com/2010/12/16/access-private-methodsfieldproperty-with-extension-method/
    /// </summary>
    public static class SystemReflectionPrivateAccessBruteForceExtensions
    {
        /// <summary>
        /// BBernard
        /// Convenience method for setting a Property Value on an object instance.
        /// NOTE: This is not the highest performance mechanism for doing this because Reflection is always used, 
        ///         and the MethodInfo is not cached!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T BruteForceGet<T>(this object obj, string name)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            Type type = obj.GetType();
            FieldInfo field = type.GetField(name, flags);
            if (field != null)
                return (T)field.GetValue(obj);

            PropertyInfo property = type.GetProperty(name, flags);
            if (property != null)
                return (T)property.GetValue(obj, null);

            return default;
        }


        /// <summary>
        /// BBernard
        /// Convenience method for setting a Property Value on an object instance.
        /// NOTE: This is not the highest performance mechanism for doing this because Reflection is always used, 
        ///         and the MethodInfo is not cached!
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void BruteForceSet(this object obj, string name, object value)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            Type type = obj.GetType();
            FieldInfo field = type.GetField(name, flags);
            if (field != null)
                field.SetValue(obj, value);

            PropertyInfo property = type.GetProperty(name, flags);
            if (property != null)
                property.SetValue(obj, value, null);

        }

        /// <summary>
        /// BBernard
        /// Convenience method for getting the MethodInfo definition of a instance method.
        /// NOTE: This is not the highest performance mechanism for doing this because Reflection is always used, 
        ///         and the MethodInfo is not cached!
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="methodName"></param>
        /// <param name="paramTypes"></param>
        /// <returns></returns>
        public static MethodInfo BruteForceFindMethod(this object obj, string methodName, IEnumerable<Type> paramTypes = null)
        {
            Type type = obj.GetType();

            //BBernard
            //NOTE: We must use Public & NonPublic methods because internal classes may be inaccessible but have public methods!
            BindingFlags flags = BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase;
            var paramTypesArray = paramTypes?.ToArray() ?? new Type[0];

            //BBernard
            //We must Find the correct Method via matching name and signature for input parameter types!
            MethodInfo methodInfo = type.GetMethod(methodName, flags, Type.DefaultBinder, paramTypesArray, null);
            return methodInfo;
        }

        /// <summary>
        /// BBernard
        /// Invoke a method by Brute Force
        /// NOTE: This is not the highest performance mechanism for doing this because Reflection is always used, 
        ///         and the MethodInfo is not cached!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="methodName"></param>
        /// <param name="paramArray"></param>
        /// <returns></returns>
        public static T BruteForceInvoke<T>(this object obj, string methodName, params object[] paramArray)
        {
            MethodInfo method = BruteForceFindMethod(obj, methodName);
            return (T)method.Invoke(obj, paramArray);
        }
    }

    /// <summary>
    /// BBernard - 2017
    /// Set of custom extensions to help working with MethodInfo Reflection objects such as:
    ///  - Accessing private elements with Brute Force approach
    ///  - Creating dynamic Delegates for High Performance invocation of functions retrieved via Reflection
    ///  - Working with Generic Methods via Reflection
    /// 
    /// NOTE: This is extremely useful when real work-arounds are needed and we do not own or want to 
    ///         copy/paste class code from
    ///         other projects/assemblies, etc.
    /// </summary>

    public static class SystemReflectionMethodExtensions
    {
        /// <summary>
        /// BBernard
        /// Helper Class for comparing Type objects with .Net Reflection
        /// Original logic based on information found here:
        ///     https://stackoverflow.com/a/4036187/7293142
        /// </summary>
        public class SimpleTypeComparer : IEqualityComparer<Type>
        {
            public bool IsObjectPlaceholderMatchingEnabled { get; }

            private readonly Type _typeOfObject = typeof(object);

            public SimpleTypeComparer(bool objectPlaceholderMatchingEnabled = false)
            {
                IsObjectPlaceholderMatchingEnabled = objectPlaceholderMatchingEnabled;
            }

            public bool Equals(Type x, Type y)
            {
                if (IsObjectPlaceholderMatchingEnabled && (x == _typeOfObject || y == _typeOfObject))
                {
                    return true;
                }
                else
                {
                    return x.Assembly == y.Assembly
                            && x.Namespace == y.Namespace
                            && x.Name == y.Name;
                }
            }

            public int GetHashCode(Type obj)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// BBernard
        /// Static Type Comparer instance used by the Extension Methods.
        /// </summary>
        private static readonly SimpleTypeComparer _simpleTypeComparer = new SimpleTypeComparer(true);

        /// <summary>
        /// BBernard
        /// Convenience method for getting the MethodInfo definition of a method using the specified Delegate or Func<> defnition stub
        /// to dynamically search for a matching signature.
        /// NOTE: The Delegate defnition stub may be null because only the Type is required.
        /// </summary>
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="type"></param>
        /// <param name="methodName"></param>
        /// <param name="flags"></param>
        /// <param name="delegateDefinitionStub"></param>
        /// <returns></returns>
        public static MethodInfo GetGenericMethodForDelegate<TDelegate>(this Type type, string methodName, BindingFlags flags, TDelegate delegateDefinitionStub)
            //where TDelegate: Delegate //Delegate Type constraint only available in C# v7.3+
            where TDelegate : class
        {
            var parameterTypeArray = delegateDefinitionStub.GetDelegateParameterTypes();
            return GetGenericMethod(type, methodName, flags, parameterTypeArray);
        }

        /// <summary>
        /// BBernard
        /// Returns MethodInfo results for Generic Methods, that are much harder to retrieve than normal methods
        /// with .Net Reflection.
        /// NOTE: Logic is optimized and enhanced, but based on original logic and detailed info. found at the following:
        ///     https://stackoverflow.com/a/5218492/7293142
        ///     https://stackoverflow.com/a/4036187/7293142
        /// </summary>
        /// <param name="type"></param>
        /// <param name="flags"></param>
        /// <param name="methodName"></param>
        /// <param name="genericArgTypes"></param>
        /// <param name="parameterTypes"></param>
        /// <param name="comparisonMode"></param>
        /// <returns></returns>
        public static MethodInfo GetGenericMethod(this Type type, string methodName, BindingFlags flags, Type[] parameterTypes)
        {
            StringComparison comparisonMode = (flags & BindingFlags.IgnoreCase) == BindingFlags.IgnoreCase
                                                ? StringComparison.OrdinalIgnoreCase
                                                : StringComparison.Ordinal;
            //BBernard
            //First find methods that match the specified Name and the exact matched method by FULL Signature 
            //  including Generic Argument Types and Parameter Types.
            var methodInfo = type.GetMethods(flags)
                                .Where(m =>
                                {
                                    var methodParameterTypes = m.GetParameters().Select(p => p.ParameterType);
                                    return m.Name.Equals(methodName, comparisonMode)
                                            && methodParameterTypes.SequenceEqual(parameterTypes, _simpleTypeComparer);
                                })
                                .FirstOrDefault();

            return methodInfo;
        }

        /// <summary>
        /// BBernard
        /// Dynamically get the Parameter Type[] from a specified Delegate or definition stub of the Delegate.
        /// NOTE: The Delegate can be null, as only the Type is needed.
        /// </summary>
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="delegateDefinitionStub"></param>
        /// <returns></returns>
        public static Type[] GetDelegateParameterTypes<TDelegate>(this TDelegate delegateDefinitionStub)
            //where TDelegate: Delegate //Delegate Type constraint only available in C# v7.3+
            where TDelegate : class
        {
            //BBernard
            //Since we don't support Delegate Type Constraint (above), we provide a runtime constraint check.
            var delegateType = typeof(TDelegate);
            if (!typeof(Delegate).IsAssignableFrom(delegateType)) throw new ArgumentException("A valid Delegate type must be specified.", nameof(delegateDefinitionStub));

            //BBernard
            //Dynamically retrieve the Types of all parameters of the specified Delegate
            //NOTE: We do this cleverly by reading the Parameters from the Invoke method of the delegate!
            //NOTE: Original logic adapted from: https://stackoverflow.com/a/429564/7293142
            MethodInfo invokeMethodInfo = delegateType.GetMethod("Invoke");
            var parameterTypeArray = invokeMethodInfo.GetParameters().Select(p => p.ParameterType).ToArray();
            return parameterTypeArray;
        }

        /// <summary>
        /// BBernard
        /// Dynamically create a Delegate for high performance execution of a method found via Reflection; this supports both 
        ///     standard Methods or Generic Method definitions!
        /// For Generic method definitions, we convert the method definition of the Generic method
        ///     into a full generic implementation using the specified Type[] array.
        /// NOTE: With the dynamic Delegate we create, Reflection will no longer be called when invoking the Function!
        /// </summary>
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="methodInfo">A Reflection based Method Info definition to use to create the dynamic Delegate from</param>
        /// <param name="delegateDefinitionStub">Stub Func<> or Delegate defining the definition of the Delegate we want to create</param>
        /// <param name="genericArgTypes">Types for each Generic Argument to support Generic Methods</param>
        /// <returns></returns>
        public static TDelegate CreateDynamicDelegate<TDelegate>(this MethodInfo methodInfo, TDelegate delegateDefinitionStub, params Type[] genericArgTypes)
            //where TDelegate: Delegate //Delegate Type constraint only available in C# v7.3+
            where TDelegate : class
        {
            //BBernard
            //Since we don't support Delegate Type Constraint (above), we provide a runtime constraint check.
            var delegateType = typeof(TDelegate);
            if (!typeof(Delegate).IsAssignableFrom(delegateType)) throw new ArgumentException("A valid Delegate type must be specified.", nameof(delegateDefinitionStub));

            //BBernard
            //Handle Generic Methods which must be converted to their non-generic strongly typed form 
            //  before Delegate can be created!
            var delegateMethodInfo = methodInfo.IsGenericMethodDefinition && genericArgTypes.Length >= 1
                                        ? methodInfo.MakeGenericMethod(genericArgTypes)
                                        : methodInfo;

            //We must create the Dynamic Delegate Type AFTER we've converted the Generic method definition 
            //  into a Generic method Implementation.
            var dynamicDelegateType = delegateMethodInfo.CreateDynamicDelegateType();

            //Once we have a full Generic method implementation (not the definition) we can create a strongly 
            //  typed Delegate for high performance Execution!
            var dynamicDelegate = delegateMethodInfo.CreateDelegate(dynamicDelegateType);
            return dynamicDelegate as TDelegate;
        }

        /// <summary>
        /// BBernard
        /// Dynamically create a Delegate for high performance execution of a method found via Reflection; this supports both 
        ///     standard Methods or Generic Method definitions!
        /// For Generic method definitions, we convert the method definition of the Generic method
        ///     into a full generic implementation using the specified Type[] array.
        /// NOTE: With the dynamic Delegate we create, Reflection will no longer be called when invoking the Function!
        /// </summary>
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="methodInfo">A Reflection based Method Info definition to use to create the dynamic Delegate from</param>
        /// <param name="genericArgTypes">Types for each Generic Argument to support Generic Methods</param>
        /// <returns></returns>
        public static Delegate CreateDynamicDelegate(this MethodInfo methodInfo, params Type[] genericArgTypes)
        {
            //BBernard
            //Handle Generic Methods which must be converted to their non-generic strongly typed form 
            //  before Delegate can be created!
            var delegateMethodInfo = methodInfo.IsGenericMethodDefinition && genericArgTypes.Length >= 1
                                        ? methodInfo.MakeGenericMethod(genericArgTypes)
                                        : methodInfo;

            //We must create the Dynamic Delegate Type AFTER we've converted the Generic method definition 
            //  into a Generic method Implementation.
            var dynamicDelegateType = delegateMethodInfo.CreateDynamicDelegateType();

            //Once we have a full Generic method implementation (not the definition) we can create a strongly 
            //  typed Delegate for high performance Execution!
            var dynamicDelegate = delegateMethodInfo.CreateDelegate(dynamicDelegateType);
            return dynamicDelegate;
        }

        /// <summary>
        /// BBernard
        /// Dynamically create the Delegate Type needed to create a Dynamic Delegate for the specified method implementation.
        /// NOTE: For Generic method definitions you usually need to first convert it to a valid strongly-typed generic method 
        ///         implementation using (MethodInfo.MakeGenericMethod()).
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <returns></returns>
        private static Type CreateDynamicDelegateType(this MethodInfo methodInfo)
        {
            //BBernard
            //Build the Type Parameters as well as the the Return Type
            var paramTypesList = methodInfo.GetParameters().Select(p => p.ParameterType).ToList();

            //Per the documentation for Expression.GetDelegateType(), the return type has to be the last argument in the List!
            paramTypesList.Add(methodInfo.ReturnType);

            //Create a dynamic Type based on the Parameter information specified
            var delegateDynamicExprType = Expression.GetDelegateType(paramTypesList.ToArray());
            return delegateDynamicExprType;
        }
    }

    /// <summary>
    /// BBernard
    /// Helper class to call methods on Static classes (e.g. no instance available to extend via normal Extension method)
    /// </summary>
    public static class StaticReflectionHelper
    {
        /// <summary>
        /// BBernard
        /// Convenience method for getting the MethodInfo definition of a Static Method using the specified Delegate or Func<> defnition stub
        ///     to dynamically search for a matching signature.
        /// NOTE: The Delegate defnition stub may be null because only the Type is required.
        /// </summary>
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="staticClassType"></param>
        /// <param name="methodName"></param>
        /// <param name="delegateDefinitionStub"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public static MethodInfo FindStaticMethodForDelegate<TDelegate>(Type staticClassType, string methodName, TDelegate delegateDefinitionStub, bool ignoreCase = true)
            //where TDelegate: Delegate //Delegate Type constraint only available in C# v7.3+
            where TDelegate : class
        {
            var parameterTypeArray = delegateDefinitionStub.GetDelegateParameterTypes();
            return FindStaticMethod(staticClassType, methodName, parameterTypeArray, ignoreCase);
        }

        /// <summary>
        /// BBernard
        /// Convenience method for getting the MethodInfo definition of a Static Method.
        /// </summary>
        /// <param name="staticClassType"></param>
        /// <param name="methodName"></param>
        /// <param name="paramTypes"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public static MethodInfo FindStaticMethod(Type staticClassType, string methodName, Type[] paramTypes = null, bool ignoreCase = true)
        {
            //BBernard
            //NOTE: We must use Public & NonPublic methods because internal classes may be inaccessible but have public methods!
            BindingFlags flags = BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            if (ignoreCase) flags |= BindingFlags.IgnoreCase;

            var paramTypesArray = paramTypes ?? Type.EmptyTypes;

            //BBernard
            //We must Find the correct Method via matching name and signature for input parameter types.
            //NOTE: We support both standard methods and Generic methods based on if a Generic Arg Types were specified.
            MethodInfo methodInfo = staticClassType.GetGenericMethod(methodName, flags, paramTypesArray);
            return methodInfo;
        }

        /// <summary>
        /// BBernard
        /// Invoke a Generic Static method by Brute Force
        /// NOTE: This is not the highest performance mechanism for doing this because Reflection is always used, 
        ///         and the MethodInfo is not cached!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="staticClassType"></param>
        /// <param name="methodName"></param>
        /// <param name="genericArgTypes"></param>
        /// <param name="paramArray"></param>
        /// <returns></returns>
        public static T BruteForceInvokeGeneric<T>(Type staticClassType, string methodName, Type[] genericArgTypes, params object[] paramArray)
        {
            var paramTypes = paramArray.Select(p => p.GetType()).ToArray();
            MethodInfo methodInfo = FindStaticMethod(staticClassType, methodName, paramTypes);

            //BBernard
            //We must convert the Generic method definition into a Generic method implementaiton before we can execute it!
            var genericMethodInfo = methodInfo.MakeGenericMethod(genericArgTypes);
            return (T)genericMethodInfo.Invoke(null, paramArray);
        }

        /// <summary>
        /// BBernard
        /// Invoke a Static method by Brute Force
        /// NOTE: This is not the highest performance mechanism for doing this because Reflection is always used, 
        ///         and the MethodInfo is not cached!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="staticClassType"></param>
        /// <param name="methodName"></param>
        /// <param name="paramArray"></param>
        /// <returns></returns>
        public static T BruteForceInvoke<T>(Type staticClassType, string methodName, params object[] paramArray)
        {
            var paramTypes = paramArray.Select(p => p.GetType()).ToArray();
            MethodInfo methodInfo = FindStaticMethod(staticClassType, methodName, paramTypes);

            return (T)methodInfo.Invoke(null, paramArray);
        }
    }
}