using System.Linq.Expressions;
using System.Reflection;
using Niddle.Injectables.Base;
using Niddle.Injectables.Expressions;

namespace Niddle.Injectables
{
    public class InjectableProperty<TTarget, TValue> : InjectableMemberBase<TTarget, TValue, PropertyInfo>
    {
        public InjectableProperty(IInjectionExpression injectionExpression, PropertyInfo propertyInfo)
            : base(injectionExpression, propertyInfo, propertyInfo.PropertyType)
        {
        }

        protected override MemberExpression GetTargetMemberExpression(PropertyInfo memberInfo, Expression target)
        {
            return Expression.Property(target, memberInfo);
        }
    }
}