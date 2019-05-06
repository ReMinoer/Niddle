using System;
using System.Collections;
using System.Collections.Generic;

namespace Niddle
{
    public interface IDependencyResolver
    {
        T Resolve<T>(object key = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyResolver resolver = null, IEnumerable<object> args = null);
        object Resolve(Type type, object key = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyResolver resolver = null, IEnumerable<object> args = null);
        IEnumerable<T> ResolveMany<T>(object key = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyResolver resolver = null, IEnumerable<object> args = null);
        IEnumerable ResolveMany(Type type, object key = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyResolver resolver = null, IEnumerable<object> args = null);
        bool TryResolve<T>(out T obj, object key = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyResolver resolver = null, IEnumerable<object> args = null);
        bool TryResolve(out object obj, Type type, object key = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyResolver resolver = null, IEnumerable<object> args = null);
        bool TryResolveMany<T>(out IEnumerable<T> objs, object key = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyResolver resolver = null, IEnumerable<object> args = null);
        bool TryResolveMany(out IEnumerable objs, Type type, object key = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyResolver resolver = null, IEnumerable<object> args = null);
    }
}