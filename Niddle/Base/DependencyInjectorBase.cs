using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Niddle.Base
{
    public abstract class DependencyInjectorBase : IDependencyInjector
    {
        public abstract object Resolve(Type type, object key = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyInjector dependencyInjector = null, IEnumerable<object> args = null);
        public abstract bool TryResolve(out object obj, Type type, object key = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyInjector dependencyInjector = null, IEnumerable<object> args = null);
        
        public virtual IEnumerable ResolveMany(Type type, object key = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyInjector dependencyInjector = null, IEnumerable<object> args = null)
        {
            yield return Resolve(type, key, origins, dependencyInjector ?? this, args);
        }

        public virtual bool TryResolveMany(out IEnumerable objs, Type type, object key = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyInjector dependencyInjector = null, IEnumerable<object> args = null)
        {
            bool result = TryResolve(out object obj, key, origins, dependencyInjector ?? this, args);
            objs = new[] { obj };
            return result;
        }

        public T Resolve<T>(object key = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyInjector dependencyInjector = null, IEnumerable<object> args = null)
            => (T)Resolve(typeof(T), key, origins, dependencyInjector ?? this, args);

        public IEnumerable<T> ResolveMany<T>(object key = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyInjector dependencyInjector = null, IEnumerable<object> args = null)
            => ResolveMany(typeof(T), key, origins, dependencyInjector ?? this, args).Cast<T>();

        public bool TryResolve<T>(out T obj, object key = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyInjector dependencyInjector = null, IEnumerable<object> args = null)
        {
            if (TryResolve(out object temp, typeof(T), key, origins, dependencyInjector ?? this, args))
            {
                obj = (T)temp;
                return true;
            }

            obj = default(T);
            return false;
        }

        public bool TryResolveMany<T>(out IEnumerable<T> objs, object key = null, InstanceOrigins origins = InstanceOrigins.All, IDependencyInjector dependencyInjector = null, IEnumerable<object> args = null)
        {
            if (TryResolveMany(out IEnumerable enumerable, typeof(T), key, origins, dependencyInjector ?? this, args))
            {
                objs = enumerable.Cast<T>();
                return true;
            }

            objs = null;
            return false;
        }
    }
}