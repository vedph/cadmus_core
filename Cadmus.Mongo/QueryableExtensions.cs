using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Cadmus.Mongo;

/// <summary>
/// Extensions to IQueryable to allow for sorting by properties known at
/// runtime only.
/// See https://stackoverflow.com/questions/41244/dynamic-linq-orderby-on-ienumerablet-iqueryablet.
/// </summary>
internal static class QueryableExtensions
{
    /// <summary>
    /// Orders by the specified property.
    /// </summary>
    /// <typeparam name="T">The queryable type.</typeparam>
    /// <param name="source">The source.</param>
    /// <param name="property">The property.</param>
    /// <returns></returns>
    public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source,
        string property)
    {
        return ApplyOrder<T>(source, property, "OrderBy");
    }

    /// <summary>
    /// Orders the by the specified property, descending.
    /// </summary>
    /// <typeparam name="T">The queryable type.</typeparam>
    /// <param name="source">The source.</param>
    /// <param name="property">The property.</param>
    /// <returns>Ordered sequence</returns>
    public static IOrderedQueryable<T> OrderByDescending<T>(
        this IQueryable<T> source,
        string property)
    {
        return ApplyOrder<T>(source, property, "OrderByDescending");
    }

    /// <summary>
    /// Orders by a subcondition.
    /// </summary>
    /// <typeparam name="T">The queryable type.</typeparam>
    /// <param name="source">The source.</param>
    /// <param name="property">The property.</param>
    /// <returns>Ordered sequence</returns>
    public static IOrderedQueryable<T> ThenBy<T>(
        this IOrderedQueryable<T> source,
        string property)
    {
        return ApplyOrder<T>(source, property, "ThenBy");
    }

    /// <summary>
    /// Orders by a subcondition, descending.
    /// </summary>
    /// <typeparam name="T">The queryable type.</typeparam>
    /// <param name="source">The source.</param>
    /// <param name="property">The property.</param>
    /// <returns>Ordered sequence</returns>
    public static IOrderedQueryable<T> ThenByDescending<T>(
        this IOrderedQueryable<T> source,
        string property)
    {
        return ApplyOrder<T>(source, property, "ThenByDescending");
    }

    static private IOrderedQueryable<T> ApplyOrder<T>(
        IQueryable<T> source,
        string property,
        string methodName)
    {
        string[] props = property.Split('.');
        Type type = typeof(T);
        ParameterExpression arg = Expression.Parameter(type, "x");
        Expression expr = arg;
        foreach (string prop in props)
        {
            // use reflection (not ComponentModel) to mirror LINQ
            PropertyInfo pi = type.GetProperty(prop)!;
            expr = Expression.Property(expr, pi);
            type = pi.PropertyType;
        }
        Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
        LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);

        object result = typeof(Queryable).GetMethods().Single(
                method => method.Name == methodName
                        && method.IsGenericMethodDefinition
                        && method.GetGenericArguments().Length == 2
                        && method.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), type)
                .Invoke(null, new object[] { source, lambda })!;
        return (IOrderedQueryable<T>)result!;
    }
}
