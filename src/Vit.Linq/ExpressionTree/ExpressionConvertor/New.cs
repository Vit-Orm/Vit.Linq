using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Vit.Linq.ExpressionTree.ComponentModel;

namespace Vit.Linq.ExpressionTree.ExpressionConvertor
{
    /// <summary>
    /// New             NewExpression           new User()
    /// MemberInit      MemberInitExpression    new { id = 2 }
    /// NewArrayInit    
    /// </summary>
    public class New : IExpressionConvertor
    {
        public ExpressionNode ConvertToData(DataConvertArgument arg, Expression expression)
        {
            ExpressionNode node = null;
            switch (expression)
            {
                case NewExpression newExp:
                    {
                        if (newExp.Type == typeof(DateTime) && arg.ReduceValue(newExp, out DateTime time))
                        {
                            return ExpressionNode.Constant(value: time, type: typeof(DateTime));
                        }
                        var (constructorArgs, constructorArgTypes) = GetConstractorArgs(arg, newExp);

                        node = ExpressionNode.New(type: expression.Type, constructorArgs: constructorArgs, memberArgs: null);
                        if (constructorArgTypes?.Any() == true) node.New_SetConstructorArgTypes(constructorArgTypes);
                        break;
                    }
                case MemberInitExpression memberInit:
                    {
                        // constructor and memberInit    MemberInitExpression{Bindings=[{name:"m3",value:v3}], NewExpression } -> new(v1,v2){m3=v3}

                        // ##1 constructorArgs
                        var (constructorArgs, constructorArgTypes) = GetConstractorArgs(arg, memberInit.NewExpression);

                        // ##2 memberArgs
                        var memberArgs = memberInit.Bindings.Select((MemberBinding binding) =>
                        {
                            if (binding is not MemberAssignment assign)
                                throw new NotSupportedException($"Unsupported MemberInitExpression Binding: {binding.GetType()}");

                            var name = assign.Member.Name;
                            var valueExp = assign.Expression;
                            var value = arg.convertService.ConvertToData(arg, valueExp);
                            return new MemberBind { name = name, value = value };
                        }).ToList();


                        node = ExpressionNode.New(type: expression.Type, constructorArgs: constructorArgs, memberArgs: memberArgs);
                        if (constructorArgTypes?.Any() == true) node.New_SetConstructorArgTypes(constructorArgTypes);
                        break;
                    }
                case NewArrayExpression newArrayExp:
                    {
                        // new [] {2, (u.id + 1)}
                        var initializers = newArrayExp.Expressions.Select(exp => arg.convertService.ConvertToData(arg, exp)).ToList();

                        node = ExpressionNode.New(type: expression.Type, constructorArgs: null, memberArgs: null);
                        node.initializers = initializers;
                        break;
                    }
                case ListInitExpression listInitExp:
                    {
                        // new List`1(2) {Void Add(Int32)(2), Void Add(Int32)((u.id + 1))}}

                        var initializers = listInitExp.Initializers.Select(initializer => arg.convertService.ConvertToData(arg, initializer.Arguments[0])).ToList();

                        node = ConvertToData(arg, listInitExp.NewExpression);
                        node.initializers = initializers;

                        break;
                    }
            }

            return node;

        }
        static (List<MemberBind> constructorArgs, Type[] constructorArgTypes) GetConstractorArgs(DataConvertArgument arg, NewExpression newExp)
        {
            List<MemberBind> constructorArgs;
            Type[] constructorArgTypes;

            if (newExp.Members == null)
            {
                // #1 constructor   NewExpression{args=[v1,v2]]} -> new(1,2)
                constructorArgs = newExp.Arguments?.Select(value => new MemberBind { name = null, value = arg.convertService.ConvertToData(arg, value) }).ToList();
                constructorArgTypes = newExp.Arguments?.Select(value => value.Type).ToArray();
            }
            else
            {
                // #2 constructor and memberInit    NewExpression{args=[v1,v2],members=["m1","m2"]} -> new(1,2)
                constructorArgs = new List<MemberBind>();
                List<Type> argTypes = new();
                for (var i = 0; i < newExp.Arguments.Count; i++)
                {
                    var name = newExp.Members[i].Name;
                    var valueExp = newExp.Arguments[i];
                    var value = arg.convertService.ConvertToData(arg, valueExp);
                    constructorArgs.Add(new MemberBind { name = name, value = value });
                    argTypes.Add(valueExp.Type);
                }
                constructorArgTypes = argTypes.ToArray();
            }
            return (constructorArgs, constructorArgTypes);
        }



        public Expression ConvertToCode(CodeConvertArgument arg, ExpressionNode data)
        {
            if (data.nodeType != NodeType.New) return null;

            ExpressionNode_New newNode = data;

            Expression objectExp;
            Type type = arg?.getResultTypeForNewNode?.Invoke(newNode)
                ?? newNode.New_GetType()
                ?? newNode.valueType?.ToType()
                ?? throw new NotSupportedException("type could not be null");


            // #1 Array
            if (type.IsArray)
            {
                var initializers = newNode.initializers?
                     .Select(initializer => arg.convertService.ToExpression(arg, initializer));
                return Expression.NewArrayInit(type.GetElementType(), initializers);
            }

            // #2 Object   (List is kind of Object)
            objectExp = ConstructObject(arg, type, newNode);

            // #3 initializers of List
            if (newNode.initializers?.Any() == true)
            {
                if (objectExp is not NewExpression newExp) throw new ArgumentException("unexpected initializers");

                var addMethod = newExp.Type.GetMethod("Add");
                var initializers = newNode.initializers
                     .Select(initializer => Expression.ElementInit(addMethod, arg.convertService.ToExpression(arg, initializer)));

                return Expression.ListInit(newExp, initializers);
            }

            return objectExp;
        }


        static Expression ConstructObject(CodeConvertArgument arg, Type type, ExpressionNode_New newNode)
        {
            // #1 constructorArgs
            var constructorArgExpressions = newNode.constructorArgs
                ?.Select(member => arg.convertService.ToExpression(arg, member.value)).ToArray()
                ?? Array.Empty<Expression>();

            var constructor = type.GetConstructor(constructorArgExpressions?.Select(member => member.Type).ToArray() ?? Type.EmptyTypes);

            var newExp = Expression.New(constructor, constructorArgExpressions);


            if (newNode.memberArgs?.Any() == true)
            {
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


            return newExp;

        }




    }
}
