using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;

namespace Vit.Linq
{
    public static class QueryableBuilder
    {
        public static IQueryable<Model> Build<Model>(Func<Expression, Type, object> QueryExecutor, object queryConfig = null)
        {
            var queryProvider = new QueryProvider(QueryExecutor);
            return new OrderedQueryable<Model>(queryProvider, queryConfig);
        }

        public static IQueryable<Model> Build<Model>(Func<Expression, Type, object> QueryExecutor, string queryName) => Build<Model>(QueryExecutor, (object)queryName);

        public static object GetQueryConfig(IQueryable query)
        {
            return (query as IQueryWithConfig)?.queryConfig;
        }


        public static Func<object, Type, bool> CompareQueryByName(string queryName)
        {
            return (query, elementType) => queryName == GetQueryConfig(query as IQueryable) as string;
        }


        internal static Type GetElementType(Type seqType)
        {
            Type enumerableType = FindIEnumerable(seqType);
            if (enumerableType == null) return seqType;
            return enumerableType.GetGenericArguments()[0];
        }

        internal static Type FindIEnumerable(Type seqType)
        {
            if (seqType == null || seqType == typeof(string))
                return null;

            if (seqType.IsArray)
                return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType());

            if (seqType.IsGenericType)
            {
                foreach (Type arg in seqType.GetGenericArguments())
                {
                    Type ienum = typeof(IEnumerable<>).MakeGenericType(arg);
                    if (ienum.IsAssignableFrom(seqType))
                    {
                        return ienum;
                    }
                }
            }

            Type[] ifaces = seqType.GetInterfaces();
            if (ifaces != null && ifaces.Length > 0)
            {
                foreach (Type iface in ifaces)
                {
                    Type ienum = FindIEnumerable(iface);
                    if (ienum != null) return ienum;
                }
            }

            if (seqType.BaseType != null && seqType.BaseType != typeof(object))
            {
                return FindIEnumerable(seqType.BaseType);
            }

            return null;
        }
    }

    interface IQueryWithConfig
    {
        object queryConfig { get; }
    }
    internal class OrderedQueryable<T> : IOrderedQueryable<T>, IQueryWithConfig
    {
        protected readonly Expression _expression;
        protected readonly QueryProvider _provider;

        public object queryConfig { get; set; }

        public OrderedQueryable(QueryProvider provider, object queryConfig = null)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _expression = Expression.Constant(this);
            this.queryConfig = queryConfig;
        }

        public OrderedQueryable(QueryProvider provider, Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            if (!typeof(IQueryable<T>).IsAssignableFrom(expression.Type))
            {
                throw new ArgumentOutOfRangeException(nameof(expression));
            }

            _provider = provider ?? throw new ArgumentNullException(nameof(provider));

            _expression = expression;
        }

        public Type ElementType => typeof(T);

        public Expression Expression => _expression;

        public IQueryProvider Provider => _provider;

        public IEnumerator<T> GetEnumerator()
        {
            return _provider.Execute<IEnumerable<T>>(_expression).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    internal class QueryProvider : IQueryProvider
    {
        Func<Expression, Type, object> QueryExecutor;


        public QueryProvider(Func<Expression, Type, object> QueryExecutor)
        {
            this.QueryExecutor = QueryExecutor;
        }
        public IQueryable CreateQuery(Expression expression)
        {
            Type elementType = QueryableBuilder.GetElementType(expression.Type);
            return (IQueryable)Activator.CreateInstance(typeof(OrderedQueryable<>).MakeGenericType(elementType), new object[] { this, expression });
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new OrderedQueryable<TElement>(this, expression);
        }

        public object Execute(Expression expression)
        {
            Type elementType = QueryableBuilder.GetElementType(expression.Type);
            return ExecuteExpression(expression, elementType);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return (TResult)ExecuteExpression(expression, typeof(TResult));
        }

        public object ExecuteExpression(Expression expression, Type type)
        {
            return QueryExecutor(expression, type);
        }
    }


}
