using System.Reflection;

namespace Diese.Injection.Factories.Data
{
    internal class FieldData : MemberData
    {
        public FieldInfo FieldInfo { get; private set; }

        public FieldData(FieldInfo fieldInfo)
            : base(fieldInfo.FieldType, fieldInfo)
        {
            FieldInfo = fieldInfo;
        }
    }
}