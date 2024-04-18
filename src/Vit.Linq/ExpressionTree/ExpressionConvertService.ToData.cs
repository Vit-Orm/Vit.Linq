
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using Vit.Linq.ExpressionTree.ComponentModel;

namespace Vit.Linq.ExpressionTree
{
    public partial class ExpressionConvertService
    {
        public class DataConvertArgument
        {
            public ExpressionConvertService convertService { get; set; }
            public List<ParamterInfo> parameters { get; set; }

            public class ParamterInfo
            {
                public object value { get; set; }
                public int hashCode => value?.GetHashCode() ?? 0;
                public string paramterName { get; set; }
            }
        }

        public ExpressionNode ConvertToData(Expression expression, Expression parameterExpression = null)
        {
            var arg = new DataConvertArgument { convertService = this };

            if (parameterExpression is ConstantExpression constant)
            {
                arg.parameters = new List<DataConvertArgument.ParamterInfo> { new DataConvertArgument.ParamterInfo { value = constant.Value } };
            }
            return ConvertToData(arg, expression);
        }

        public ExpressionNode ConvertToData(LambdaExpression expression)
        {
            return ConvertToData(new DataConvertArgument { convertService = this }, expression);
        }

        public ExpressionNode ConvertToData(DataConvertArgument arg, Expression expression)
        {
            if (expression is ConstantExpression constant)
            {
                if (constant.Value != null)
                {
                    var hashCode = constant.Value.GetHashCode();
                    var param = arg.parameters?.FirstOrDefault(p => p.hashCode == hashCode);
                    if (param != null)
                    {
                        return ExpressionNode.Member(parameterName: param.paramterName);
                    }
                }
                return ExpressionNode.Constant(value: constant.Value, type: expression.Type);
            }
            else if (expression is IndexExpression index)
            {
                var left = ConvertToData(arg, index.Object);
                var right = ConvertToData(arg, index.Arguments[0]);
                return ExpressionNode.ArrayIndex(left: left, right: right);
            }
            else if (expression is ParameterExpression parameter)
            {
                return ExpressionNode.Member(parameterName: parameter.Name);
            }
            else if (expression is MemberExpression member)
            {
                return ConvertExpression(arg, member);
            }
            else if (expression is BinaryExpression binary)
            {
                return ConvertExpression(arg, binary);
            }
            else if (expression is UnaryExpression unary)
            {
                return ConvertExpression(arg, unary);
            }
            else if (expression is LambdaExpression lambda)
            {
                var parameterNames = lambda.Parameters.Select(p => p.Name).ToArray();
                var body = ConvertToData(arg, lambda.Body);
                return ExpressionNode.Lambda(parameterNames: parameterNames, body: body);
            }
            else if (expression is MethodCallExpression methodCall)
            {
                return ConvertExpression(arg, methodCall);
            }
            throw new NotSupportedException($"Unsupported expression type:{expression.GetType()}");
        }

        #region Member
        ExpressionNode ConvertExpression(DataConvertArgument arg, MemberExpression member)
        {
            var name = member.Member.Name;
            if (member.Expression == null)
                throw new NotSupportedException($"Unsupported MemberExpression : {member.Member?.Name}");

            if (member.Expression is ParameterExpression parameter)
            {
                return ExpressionNode.Member(memberName: name, parameterName: parameter.Name);
            }

            var objectValue = ConvertToData(arg, member.Expression);
            if (objectValue.nodeType == NodeType.Constant)
            {
                var value = GetValue(arg, member);
                return ExpressionNode.Constant(value: value, type: member.Type);
            }
            return ExpressionNode.Member(memberName: name, objectValue: objectValue);
        }


        object GetValue(DataConvertArgument arg, Expression expression)
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
            else if (expression is LambdaExpression lambda)
            {
                var del = lambda.Compile();
                var value = del.DynamicInvoke();
                return value;
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
            else if (expression is MethodCallExpression methodCall)
            {
                var del = Expression.Lambda(methodCall).Compile();
                var value = del.DynamicInvoke();
                return value;
            }
            else if (expression is BinaryExpression binary)
            {
                if (ExpressionType.ArrayIndex == binary.NodeType)
                {
                    var array = GetValue(arg, binary.Left) as IEnumerable;
                    var index = GetValue(arg, binary.Right) as int? ?? 0;
                    var enumerator = array.GetEnumerator();
                    for (var t = 0; t <= index; t++) { enumerator.MoveNext(); }
                    var value = enumerator.Current;
                    return value;
                }
            }
            throw new NotSupportedException($"GetValue failed, Unsupported expression type : {expression.GetType()}");
        }

        string GetMemberName(DataConvertArgument arg, Expression expression)
        {
            if (expression is ParameterExpression parameter)
            {
                // top level, no need to return parameterName
                return null;
                // return parameter.Name
            }
            else if (expression is MemberExpression member)
            {
                // get nested member
                var name = member.Member.Name;
                if (member.Expression == null) return name;
                string parentName = GetMemberName(arg, member.Expression);
                return parentName == null ? name : $"{parentName}.{name}";
            }
            else if (expression is UnaryExpression unary)
            {
                if (ExpressionType.Quote == unary.NodeType)
                {
                    return GetMemberName(arg, unary.Operand);
                }
            }
            else if (expression is LambdaExpression lambda)
            {
                return GetMemberName(arg, lambda.Body);
            }

            throw new NotSupportedException($"GetMemberName failed, Unsupported expression type: {expression.GetType()}");
        }
        #endregion

        ExpressionNode ConvertExpression(DataConvertArgument arg, UnaryExpression unary)
        {
            switch (unary.NodeType)
            {
                case ExpressionType.Not:
                    return ExpressionNode.Not(body: ConvertToData(unary.Operand));
                case ExpressionType.Convert:
                    return ExpressionNode.Convert(valueType: ComponentModel.ValueType.FromType(unary.Type), body: ConvertToData(unary.Operand));
                case ExpressionType.Quote:
                    return ConvertToData(arg, unary.Operand);
            }

            throw new NotSupportedException($"Unsupported unary NodeType : {unary.NodeType}");
        }

        ExpressionNode ConvertExpression(DataConvertArgument arg, BinaryExpression binary)
        {
            var left = ConvertToData(arg, binary.Left);
            var right = ConvertToData(arg, binary.Right);

            if (left?.nodeType == NodeType.Constant && right?.nodeType == NodeType.Constant)
            {
                var value = GetValue(arg, binary);
                return ExpressionNode.Constant(value: value, type: binary.Type);
            }
            switch (binary.NodeType)
            {
                case ExpressionType.Equal: return ExpressionNode.Equal(left: left, right: right);
                case ExpressionType.NotEqual: return ExpressionNode.NotEqual(left: left, right: right);
                case ExpressionType.GreaterThan: return ExpressionNode.GreaterThan(left: left, right: right);
                case ExpressionType.GreaterThanOrEqual: return ExpressionNode.GreaterThanOrEqual(left: left, right: right);
                case ExpressionType.LessThan: return ExpressionNode.LessThan(left: left, right: right);
                case ExpressionType.LessThanOrEqual: return ExpressionNode.LessThanOrEqual(left: left, right: right);
                case ExpressionType.AndAlso: return ExpressionNode.And(left: left, right: right);
                case ExpressionType.OrElse: return ExpressionNode.Or(left: left, right: right);
                case ExpressionType.ArrayIndex: return ExpressionNode.ArrayIndex(left: left, right: right);
            }
            throw new NotSupportedException($"Unsupported binary NodeType : {binary.NodeType}");
        }
    }
}
