using System.Collections;
using System.Reflection;

namespace Niddle.Utils
{
    static public class InjectionReflectionInfos
    {
        static public readonly MethodInfo GetEnumeratorMethodInfo;
        static public readonly MethodInfo EnumeratorMoveNextMethodInfo;
        static public readonly PropertyInfo EnumeratorCurrentMethodInfo;
        static public readonly MethodInfo GetNextMethodInfo;

        static InjectionReflectionInfos()
        {
            GetEnumeratorMethodInfo = typeof(IEnumerable).GetTypeInfo().GetDeclaredMethod(nameof(IEnumerable.GetEnumerator));
            EnumeratorMoveNextMethodInfo = typeof(IEnumerator).GetTypeInfo().GetDeclaredMethod(nameof(IEnumerator.MoveNext));
            EnumeratorCurrentMethodInfo = typeof(IEnumerator).GetTypeInfo().GetDeclaredProperty(nameof(IEnumerator.Current));
            GetNextMethodInfo = typeof(InjectionReflectionInfos).GetTypeInfo().GetDeclaredMethod(nameof(GetNext));
        }

        static private object GetNext(IEnumerator enumerator)
        {
            enumerator.MoveNext();
            return enumerator.Current;
        }
    }
}