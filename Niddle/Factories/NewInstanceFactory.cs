using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Niddle.Base;
using Niddle.Exceptions;
using Niddle.Factories.Base;
using Niddle.Factories.Data;

namespace Niddle.Factories
{
    public class NewInstanceFactory : DependencyFactoryBase
    {
        static private readonly Stack<IDependencyFactory> FactoryStack = new Stack<IDependencyFactory>();
        private readonly ConstructorData _constructorData;
        private readonly PropertyData[] _propertiesData;
        private readonly FieldData[] _fieldsData;
        private bool _alreadyInvoke;
        public override InstanceOrigin? InstanceOrigin => Niddle.InstanceOrigin.Instantiation;

        public NewInstanceFactory(Type type, object serviceKey, ConstructorData constructorData, Substitution substitution)
            : base(type, serviceKey, substitution)
        {
            _constructorData = constructorData;
            
            TypeInfo attributeTypeInfo = typeof(InjectableAttributeBase).GetTypeInfo();

            PropertyInfo[] propertyInfos = type.GetRuntimeProperties()
                .Where(x => (x.SetMethod != null && x.SetMethod.IsPublic || x.GetMethod != null && x.GetMethod.IsPublic) && x.CustomAttributes.Any(y => attributeTypeInfo.IsAssignableFrom(y.AttributeType.GetTypeInfo())))
                .ToArray();

            _propertiesData = new PropertyData[propertyInfos.Length];
            for (int i = 0; i < _propertiesData.Length; i++)
                _propertiesData[i] = new PropertyData(propertyInfos[i]);

            FieldInfo[] fieldInfos = type.GetRuntimeFields()
                .Where(x => x.IsPublic && x.CustomAttributes.Any(y => attributeTypeInfo.IsAssignableFrom(y.AttributeType.GetTypeInfo())))
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

            var parameters = new object[_constructorData.ParametersData.Length];
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

            object instance = _constructorData.Delegate.DynamicInvoke(parameters);

            foreach (FieldData field in _fieldsData)
            {
                if (field.InjectableAttribute is IInjectableManyAttribute manyAttribute)
                {
                    foreach (object value in manyAttribute.GetInjectableTypes(field.Type).SelectMany(x =>
                    {
                        injector.TryResolveMany(out IEnumerable values, x, field.InjectableAttribute, field.ServiceKey);
                        return values.Cast<object>();
                    }))
                    {
                        manyAttribute.Inject(field.FieldInfo, instance, value);
                    }
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
                    foreach (object value in manyAttribute.GetInjectableTypes(property.Type).SelectMany(x =>
                    {
                        injector.TryResolveMany(out IEnumerable values, x, property.InjectableAttribute, property.ServiceKey);
                        return values.Cast<object>();
                    }))
                    {
                        manyAttribute.Inject(property.PropertyInfo, instance, value);
                    }
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