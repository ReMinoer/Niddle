﻿using System;
using System.Linq.Expressions;
using System.Reflection;
using Niddle.Injectables.Base;
using Niddle.Injectables.Expressions;

namespace Niddle.Injectables
{
    public class InjectableField<TTarget, TValue> : InjectableMemberBase<TTarget, TValue, FieldInfo>
    {
        public InjectableField(IInjectionExpression injectionExpression, FieldInfo fieldInfo, Delegate targetGetterDelegate = null)
            : base(injectionExpression, fieldInfo, fieldInfo.FieldType, targetGetterDelegate)
        {
        }

        protected override MemberExpression GetTargetMemberExpression(FieldInfo memberInfo, Expression target)
        {
            return Expression.Field(target, memberInfo);
        }
    }
}