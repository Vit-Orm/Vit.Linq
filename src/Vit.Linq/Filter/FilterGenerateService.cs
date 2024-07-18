using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Vit.Linq;

using Vit.Linq.Filter.ComponentModel;
using Vit.Linq.Filter.FilterGenerator;
using Vit.Linq.Filter.MethodCalls;

namespace Vit.Linq.Filter
{
    public partial class FilterGenerateService
    {

        public static FilterGenerateService Instance = new FilterGenerateService();

        protected List<IFilterGenerator> filterGenerators = new();


        public virtual void AddGenerator(IFilterGenerator generator)
        {
            filterGenerators.Add(generator);
            filterGenerators.Sort((a, b) => a.priority - b.priority);
        }


        public virtual bool RegisterMethodConvertor(IMethodConvertor convertor)
        {
            var methodCallConvertor = filterGenerators.FirstOrDefault(m => m is MethodCall) as MethodCall;
            if (methodCallConvertor == null) return false;
            methodCallConvertor.RegisterMethodConvertor(convertor);
            return true;
        }

        public static List<Type> defaultGeneratorTypes;

        public FilterGenerateService()
        {
            // populate
            defaultGeneratorTypes ??=
                    typeof(FilterGenerateService).Assembly.GetTypes().Where(type => type.IsClass
                        && !type.IsAbstract
                        && typeof(IFilterGenerator).IsAssignableFrom(type)
                        && type.GetConstructor(Type.EmptyTypes) != null
                    ).ToList();
            defaultGeneratorTypes.ForEach(type => AddGenerator(Activator.CreateInstance(type) as IFilterGenerator));
        }


        public virtual FilterRule ConvertToData(FilterGenerateArgument arg, Expression expression)
        {
            foreach (var filterGenerator in filterGenerators)
            {
                var filter = filterGenerator.ConvertToData(arg, expression);
                if (filter != null) return filter;
            }

            throw new NotSupportedException($"Unsupported expression type: {expression.GetType()}");
        }


        public virtual FilterRule ConvertToData(Expression expression)
        {
            return ConvertToQueryAction(expression).filter;
        }

        public virtual QueryAction ConvertToQueryAction(Expression expression)
        {
            QueryAction queryAction = new();

            var arg = new FilterGenerateArgument { convertService = this, queryAction = queryAction };
            queryAction.filter = ConvertToData(arg, expression);

            return queryAction;
        }



        public virtual object GetValue(Expression expression)
        {
            if (expression is ConstantExpression constant)
            {
                return constant.Value;
            }
            else if (expression is UnaryExpression unary)
            {
                if (ExpressionType.Convert == unary.NodeType)
                {
                    var del = Expression.Lambda(unary).Compile();
                    var value = del.DynamicInvoke();
                    return value;
                }
            }
            else if (expression is MemberExpression member)
            {
                if (ExpressionType.MemberAccess == member.NodeType)
                {
                    var del = Expression.Lambda(member).Compile();
                    var value = del.DynamicInvoke();
                    return value;
                }
            }
            throw new NotSupportedException($"GetValue failed, Unsupported expression type: {expression.GetType()}");
        }


        public virtual string GetMemberName(Expression expression)
        {
            if (expression is ParameterExpression parameter)
            {
                // top level, no need to return parameterName
                return null;
                //return parameter.Name;
            }
            else if (expression is MemberExpression member)
            {
                // get nested member
                var name = member.Member.Name;
                if (member.Expression == null) return name;
                string parentName = GetMemberName(member.Expression);
                return parentName == null ? name : $"{parentName}.{name}";
            }
            else if (expression is UnaryExpression unary)
            {
                if (ExpressionType.Quote == unary.NodeType)
                {
                    return GetMemberName(unary.Operand);
                }
            }
            else if (expression is LambdaExpression lambda)
            {
                return GetMemberName(lambda.Body);
            }

            throw new NotSupportedException($"GetMemberName failed, Unsupported expression type: {expression.GetType()}");
        }




    }
}
