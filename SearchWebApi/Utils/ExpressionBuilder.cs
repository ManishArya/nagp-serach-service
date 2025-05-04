using System.Linq.Expressions;

namespace SearchWebApi.Utils
{
    public class ExpressionBuilder
    {
        public static Expression<Func<TInput, TResult>> GetExpression<TInput, TResult>(string propertyName)
        {
            var parameterExpression = Expression.Parameter(typeof(TInput), "p");
            var memberExpression = Expression.PropertyOrField(parameterExpression, propertyName);
            var unaryExpression = Expression.Convert(memberExpression, typeof(TResult));
            return Expression.Lambda<Func<TInput, TResult>>(unaryExpression, parameterExpression);
        }
    }
}
