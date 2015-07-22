using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Diese.Injection.Exceptions;
using Diese.Injection.Factories.Data;

namespace Diese.Injection.Factories
{
    internal class TransientFactory : IDependencyFactory
    {
        static private readonly Stack<IDependencyFactory> FactoryStack = new Stack<IDependencyFactory>();
        private readonly ConstructorData _constructorData;
        private readonly PropertyData[] _propertiesData;
        private readonly FieldData[] _fieldsData;
        private bool _alreadyInvoke;
        public Type Type { get; private set; }
        public object ServiceKey { get; private set; }

        public TransientFactory(Type type, object serviceKey, ConstructorInfo constructorInfo)
        {
            Type = type;
            ServiceKey = serviceKey;

            _constructorData = new ConstructorData(constructorInfo);

            PropertyInfo[] propertyInfos = type.GetProperties()
                .Where(
                    x =>
                        x.SetMethod != null && x.SetMethod.IsPublic &&
                        x.CustomAttributes.Any(attr => attr.AttributeType == typeof(InjectableAttribute)))
                .ToArray();

            _propertiesData = new PropertyData[propertyInfos.Count()];
            for (int i = 0; i < _propertiesData.Length; i++)
                _propertiesData[i] = new PropertyData(propertyInfos[i]);

            FieldInfo[] fieldInfos = type.GetFields()
                .Where(
                    x => x.IsPublic && x.CustomAttributes.Any(attr => attr.AttributeType == typeof(InjectableAttribute)))
                .ToArray();

            _fieldsData = new FieldData[fieldInfos.Count()];
            for (int i = 0; i < _fieldsData.Length; i++)
                _fieldsData[i] = new FieldData(fieldInfos[i]);
        }

        public virtual object Get(IDependencyInjector injector)
        {
            if (_alreadyInvoke)
                throw new CyclicDependencyException(FactoryStack);

            FactoryStack.Push(this);
            _alreadyInvoke = true;

            var parameters = new object[_constructorData.Count];
            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterData data = _constructorData.ParametersData[i];
                parameters[i] = injector.Resolve(data.Type, data.ServiceKey);
            }

            object instance = _constructorData.ConstructorInfo.Invoke(parameters);

            foreach (FieldData field in _fieldsData)
                try
                {
                    field.FieldInfo.SetValue(instance, injector.Resolve(field.Type, field.ServiceKey));
                }
                catch (NotRegisterException)
                {
                }

            foreach (PropertyData property in _propertiesData)
                try
                {
                    property.PropertyInfo.SetValue(instance, injector.Resolve(property.Type, property.ServiceKey));
                }
                catch (NotRegisterException)
                {
                }

            _alreadyInvoke = false;
            FactoryStack.Pop();

            return instance;
        }
    }
}