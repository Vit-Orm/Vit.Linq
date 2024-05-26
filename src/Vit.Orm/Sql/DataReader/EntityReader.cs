using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

using Vit.Linq.ExpressionTree;
using Vit.Linq.ExpressionTree.ComponentModel;

namespace Vit.Orm.Sql.DataReader
{
    public class EntityReader : IDbDataReader
    {
        public List<string> sqlFields { get; private set; } = new List<string>();

        Type entityType;
        List<IArgReader> entityArgReaders = new List<IArgReader>();
        Delegate lambdaCreateEntity;


        public string BuildSelect(Type entityType, ISqlTranslator sqlTranslator, ExpressionConvertService convertService, ExpressionNode selectedFields)
        {
            this.entityType = entityType;

            var cloner = new ExpressionNodeCloner();
            cloner.clone = (node) =>
            {
                if (node?.nodeType == NodeType.Member)
                {
                    ExpressionNode_Member member = node;

                    var argName = GetArgument(sqlTranslator, member);

                    if (argName != null)
                    {
                        return (true, ExpressionNode.Member(parameterName: argName, memberName: null));
                    }
                }
                return default;
            };
            ExpressionNode_New newExp = cloner.Clone(selectedFields);


            #region Compile Lambda
            var lambdaNode = ExpressionNode.Lambda(entityArgReaders.Select(m => m.argName).ToArray(), (ExpressionNode)newExp);
            //var strNode = Json.Serialize(lambdaNode);

            var lambdaExp = convertService.ToLambdaExpression(lambdaNode, entityArgReaders.Select(m => m.argType).ToArray());

            lambdaCreateEntity = lambdaExp.Compile();
            #endregion

            // sqlFields
            return String.Join(",", sqlFields);
        }


        public object ReadData(IDataReader reader)
        {
            return new Func<IDataReader, List<string>>(ReadEntity<string>)
              .GetMethodInfo().GetGenericMethodDefinition().MakeGenericMethod(entityType)
              .Invoke(this, new object[] { reader });
        }

        List<Entity> ReadEntity<Entity>(IDataReader reader)
        {
            var list = new List<Entity>();

            while (reader.Read())
            {
                var lambdaArgs = entityArgReaders.Select(m => m.Read(reader)).ToArray();
                var obj = (Entity)lambdaCreateEntity.DynamicInvoke(lambdaArgs);
                list.Add(obj);
            }
            return list;
        }

        string GetArgument(ISqlTranslator sqlTranslator, ExpressionNode_Member member)
        {
            // tableName_fieldName   tableName_
            var argUniqueKey = $"arg_{member.objectValue?.parameterName ?? member.parameterName}_{member.memberName}";

            IArgReader argReader = entityArgReaders.FirstOrDefault(reader => reader.argUniqueKey == argUniqueKey);

            if (argReader == null)
            {
                var argName = "arg_" + entityArgReaders.Count;
                if (member.memberName != null)
                {
                    // Value arg
                    string sqlFieldName = sqlTranslator.GetSqlField(member);
                    argReader = new ValueReader(this, member, argUniqueKey, argName, sqlFieldName);
                }
                else
                {
                    // Entity arg
                    var argType = member.Member_GetType();

                    argReader = new ModelReader(this, sqlTranslator, member, argUniqueKey, argName, argType);
                }
                entityArgReaders.Add(argReader);
            }
            return argReader.argName;
        }


    }
}
