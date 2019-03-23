using System;
using System.Linq;
using System.Reflection;

namespace Niddle.Builders.Helpers
{
    static public class ReflectionHelper
    {
        static public ConstructorInfo GetDefaultConstructor(Type type)
        {
            return type.GetTypeInfo()
                       .DeclaredConstructors
                       .Where(x => x.IsPublic)
                       .Aggregate((min, next) => next.GetParameters().Length < min.GetParameters().Length ? next : min);
        }
    }
}