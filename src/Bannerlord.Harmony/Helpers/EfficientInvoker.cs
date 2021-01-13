using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

// https://github.com/tdupont750/tact.net/blob/9d73a912dfd64bbd7fa88d3d1460c23c848af61a/framework/src/Tact/Reflection/EfficientInvoker.cs
namespace Tact.Reflection
{
    public sealed class EfficientInvoker
    {
        private static readonly ConcurrentDictionary<ConstructorInfo, Func<object[], object>> ConstructorToWrapperMap = new();

        public static Func<object[], object> ForConstructor(ConstructorInfo constructor)
        {
            if (constructor == null)
                throw new ArgumentNullException(nameof(constructor));

            return ConstructorToWrapperMap.GetOrAdd(constructor, t =>
            {
                CreateParamsExpressions(constructor, out ParameterExpression argsExp, out Expression[] paramsExps);

                var newExp = Expression.New(constructor, paramsExps);
                var resultExp = Expression.Convert(newExp, typeof(object));
                var lambdaExp = Expression.Lambda(resultExp, argsExp);
                var lambda = lambdaExp.Compile();
                return (Func<object[], object>)lambda;
            });
        }

        private static void CreateParamsExpressions(MethodBase method, out ParameterExpression argsExp, out Expression[] paramsExps)
        {
            var parameters = method.GetParameters().Select(p => p.ParameterType).ToArray();

            argsExp = Expression.Parameter(typeof(object[]), "args");
            paramsExps = new Expression[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                var constExp = Expression.Constant(i, typeof(int));
                var argExp = Expression.ArrayIndex(argsExp, constExp);
                paramsExps[i] = Expression.Convert(argExp, parameters[i]);
            }
        }
    }
}