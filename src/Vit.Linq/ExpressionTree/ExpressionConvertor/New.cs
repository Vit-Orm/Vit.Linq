using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;

using Vit.Linq.ExpressionTree.ComponentModel;

namespace Vit.Linq.ExpressionTree.ExpressionConvertor
{
    public class New : IExpressionConvertor
    {

        List<MemberBind> GetConstractorArgs(DataConvertArgument arg, NewExpression newExp)
        {
            List<MemberBind> constructorArgs;

            if (newExp.Members == null)
            {
                // #1 constructor   NewExpression{args=[v1,v2]]} -> new(1,2)
                constructorArgs = newExp.Arguments?.Select(value => new MemberBind { name = null, value = arg.convertService.ConvertToData(arg, value) }).ToList();
            }
            else
            {
                // #2 constructor and memberInit    NewExpression{args=[v1,v2],members=["m1","m2"]} -> new(1,2)
                constructorArgs = new List<MemberBind>();
                for (var i = 0; i < newExp.Arguments.Count; i++)
                {
                    var name = newExp.Members[i].Name;
                    var valueExp = newExp.Arguments[i];
                    var value = arg.convertService.ConvertToData(arg, valueExp);
                    constructorArgs.Add(new MemberBind { name = name, value = value });
                }
            }
            return constructorArgs;
        }


        public ExpressionNode ConvertToData(DataConvertArgument arg, Expression expression)
        {
            ExpressionNode node = null;
            if (expression is NewExpression newExp)
            {
                if (newExp.Type == typeof(DateTime) && arg.ReduceValue(newExp, out DateTime time))
                {
                    return ExpressionNode.Constant(value: time, type: typeof(DateTime));
                }
                List<MemberBind> constructorArgs = GetConstractorArgs(arg, newExp);
                node = ExpressionNode.New(constructorArgs: constructorArgs, memberArgs: null);
            }
            else if (expression is MemberInitExpression memberInit)
            {
                // #3 constructor and memberInit    MemberInitExpression{Bindings=[{name:"m3",value:v3}],NewExpression} -> new(v1,v2){m3=v3}

                // ##1 constructorArgs
                List<MemberBind> constructorArgs = GetConstractorArgs(arg, memberInit.NewExpression);

                // ##2 memberArgs
                var memberArgs = new List<MemberBind>();
                foreach (MemberBinding binding in memberInit.Bindings)
                {
                    if (binding is MemberAssignment assign)
                    {
                        var name = assign.Member.Name;
                        var valueExp = assign.Expression;
                        var value = arg.convertService.ConvertToData(arg, valueExp);
                        memberArgs.Add(new MemberBind { name = name, value = value });
                        continue;
                    }
                    throw new NotSupportedException($"Unsupported MemberInitExpression Binding: {binding.GetType()}");
                }
                node = ExpressionNode.New(constructorArgs: constructorArgs, memberArgs: memberArgs);
            }

            if (node != null)
            {
                node.New_SetType(expression.Type);
            }
            return node;
        }

        public Expression ConvertToCode(CodeConvertArgument arg, ExpressionNode data)
        {
            if (data.nodeType != NodeType.New) return null;

            ExpressionNode_New newNode = data;


            // #1 constructorArgs
            var constructorArgExpressions = newNode.constructorArgs
                ?.Select(member => arg.convertService.ToExpression(arg, member.value)).ToArray();
            constructorArgExpressions ??= new Expression[0];

            Type type = arg?.getResultTypeForNewNode?.Invoke(newNode);
            if (type == null)
            {
                type = newNode.New_GetType();
            }
            if (type == null)
            {
                throw new NotSupportedException("type could not be null");
            }

            var constructor = type.GetConstructor(constructorArgExpressions?.Select(member => member.Type).ToArray() ?? Type.EmptyTypes);

            var newExp = Expression.New(constructor, constructorArgExpressions);

            if (newNode.memberArgs?.Any() != true) return newExp;

            // #2 memberArgs
            var memberArgs = newNode.memberArgs.Select(
                 member => new { member.name, value = arg.convertService.ToExpression(arg, member.value) }
             ).ToList();

            var bindings = memberArgs.Select(member =>
            {
                var property = type.GetMember(member.name)?.FirstOrDefault();
                //property ??= type.GetProperty(member.name);
                return Expression.Bind(property, member.value);
            }).ToArray();

            return Expression.MemberInit(newExp, bindings);
        }




    }
}
