using System;
using System.Linq.Expressions;

namespace Simple.Wpf.Composition.Core.Infrastructure
{
    public static class ExpressionHelper
    {
        public static string Name<T>(Expression<Func<T>> expression)
        {
            var lambda = expression as LambdaExpression;
            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression unaryExpression)
            {
                memberExpression = (MemberExpression) unaryExpression.Operand;
            }
            else
            {
                memberExpression = (MemberExpression) lambda.Body;
            }

            return memberExpression.Member.Name;
        }
    }
}