using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Diese.Injection.Exceptions;
using Diese.Injection.Factories.Base;
using Diese.Injection.Factories.Data;

namespace Diese.Injection.Factories
{
    internal class TransientFactory : DependencyFactoryBase
    {
        static private readonly Stack<IDependencyFactory> FactoryStack = new Stack<IDependencyFactory>();
        private readonly ConstructorData _constructorData;
        private readonly PropertyData[] _propertiesData;
        private readonly FieldData[] _fieldsData;
        private bool _alreadyInvoke;

        public TransientFactory(Type type, object serviceKey, ConstructorInfo constructorInfo, Substitution substitution)
            : base(type, serviceKey, substitution)
        {
            _constructorData = new ConstructorData(constructorInfo);

            PropertyInfo[] propertyInfos = type.GetProperties()
                .Where(x => (x.SetMethod != null && x.SetMethod.IsPublic || x.GetMethod != null && x.GetMethod.IsPublic) && x.CustomAttributes.Any(y => typeof(InjectableAttributeBase).IsAssignableFrom(y.AttributeType)))
                .ToArray();

            _propertiesData = new PropertyData[propertyInfos.Length];
            for (int i = 0; i < _propertiesData.Length; i++)
                _propertiesData[i] = new PropertyData(propertyInfos[i]);

            FieldInfo[] fieldInfos = type.GetFields()
                .Where(x => x.IsPublic && x.CustomAttributes.Any(y => typeof(InjectableAttributeBase).IsAssignableFrom(y.AttributeType)))
                .ToArray();

            _fieldsData = new FieldData[fieldInfos.Length];
            for (int i = 0; i < _fieldsData.Length; i++)
                _fieldsData[i] = new FieldData(fieldInfos[i]);
        }

        public override object Get(IDependencyInjector injector)
        {
            if (_alreadyInvoke)
                throw new CyclicDependencyException(FactoryStack);

            FactoryStack.Push(this);
            _alreadyInvoke = true;

            var parameters = new object[_constructorData.Count];
            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterData data = _constructorData.ParametersData[i];
                
                if (data.HasDefaultValue)
                {
                    if (!injector.TryResolve(out parameters[i], data.Type, data.InjectableAttribute, data.ServiceKey))
                        parameters[i] = data.DefaultValue;
                }
                else
                    parameters[i] = injector.Resolve(data.Type, data.InjectableAttribute, data.ServiceKey);
            }

            object instance = _constructorData.ConstructorInfo.Invoke(parameters);

            foreach (FieldData field in _fieldsData)
            {
                if (field.InjectableAttribute is IInjectableManyAttribute manyAttribute)
                {
                    injector.TryResolveMany(out IEnumerable values, manyAttribute.GetInjectedType(field.Type), field.InjectableAttribute, field.ServiceKey);
                    manyAttribute.Inject(field.FieldInfo, instance, values);
                }
                else
                {
                    injector.TryResolve(out object value, field.Type, field.InjectableAttribute, field.ServiceKey);
                    field.InjectableAttribute.Inject(field.FieldInfo, instance, value);
                }
            }

            foreach (PropertyData property in _propertiesData)
            {
                if (property.InjectableAttribute is IInjectableManyAttribute manyAttribute)
                {
                    injector.TryResolveMany(out IEnumerable values, manyAttribute.GetInjectedType(property.Type), property.InjectableAttribute, property.ServiceKey);
                    manyAttribute.Inject(property.PropertyInfo, instance, values);
                }
                else
                {
                    injector.TryResolve(out object value, property.Type, property.InjectableAttribute, property.ServiceKey);
                    property.InjectableAttribute.Inject(property.PropertyInfo, instance, value);
                }
            }

            _alreadyInvoke = false;
            FactoryStack.Pop();

            return instance;
        }
    }
}