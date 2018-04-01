using System.Reflection;

namespace Niddle.Factories.Data
{
    internal class PropertyData : MemberData
    {
        public PropertyInfo PropertyInfo { get; private set; }

        public PropertyData(PropertyInfo propertyInfo)
            : base(propertyInfo.PropertyType, propertyInfo)
        {
            PropertyInfo = propertyInfo;
        }
    }
}