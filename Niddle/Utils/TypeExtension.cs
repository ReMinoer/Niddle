using System;
using System.Collections.Generic;
using System.Reflection;

namespace Niddle.Utils
{
    static public class TypeExtension
    {
        static public IEnumerable<Type> GetBaseTypesExclusive(this Type type)
        {
            for (Type current = type.GetTypeInfo().BaseType; current != null; current = current.GetTypeInfo().BaseType)
                yield return current;
        }
    }
}