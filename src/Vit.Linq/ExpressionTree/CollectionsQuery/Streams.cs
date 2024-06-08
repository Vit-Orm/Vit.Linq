using System;
using System.Collections.Generic;
using System.Linq;

using Vit.Linq.ExpressionTree.ComponentModel;

namespace Vit.Linq.ExpressionTree.CollectionsQuery
{
    public interface IStream
    {
        string alias { get; }
    }

    public class SourceStream : IStream
    {
        public SourceStream(object source, string alias)
        {
            this.source = source;
            this.alias = alias;
        }
        public string alias { get; private set; }
        private object source;

        public int? hashCode
        {
            get => source?.GetHashCode();
        }

        public object GetSource() => source;

        public Type GetEntityType() => source?.GetType().GenericTypeArguments[0];
    }


    public enum EJoinType
    {
        LeftJoin, InnerJoin
    }
    public class StreamToJoin
    {
        // LeftJoin , InnerJoin
        public EJoinType joinType { get; set; }
        public IStream right { get; set; }

        //  a1.id==b2.id
        public ExpressionNode on { get; set; }
    }


    /* //sql
    select u.id, u.name, u.birth,u.fatherId ,u.motherId,    father.name,  mother.name
    from `User` u
    inner join `User` father on u.fatherId = father.id 
    left join `User` mother on u.motherId = mother.id
    where u.id>1
    limit 1,5;
     */

    /* //linq
value(Vit.Linq.Converter.OrderedQueryable`1[Vit.Linq.MsTest.Converter.Join_Test+User])
.SelectMany(
     user => value(Vit.Linq.MsTest.Converter.Join_Test+<>c__DisplayClass0_1).users
             .Where(father => (Convert(father.id, Nullable`1) == user.fatherId)).DefaultIfEmpty(),
     (user, father) => new <>f__AnonymousType4`2(user = user, father = father)
 ).SelectMany(
     item => value(Vit.Linq.MsTest.Converter.Join_Test+<>c__DisplayClass0_1).users
                 .Where(mother => (Convert(mother.id, Nullable`1) == item.user.fatherId)).DefaultIfEmpty(),
     (item, mother) => new <>f__AnonymousType5`3(user = item.user, father = item.father, mother = mother)
 )
.Skip().Take().Select()
     */

    public class SelectedFields
    {
        // root value of ExpressionNode_Member is IStream
        public ExpressionNode_New fields;

        public bool? isDefaultSelect { get; set; }
        internal bool TryGetField(string fieldName, out ExpressionNode field)
        {
            field = null;

            var fieldInfo = fields?.memberArgs?.FirstOrDefault(m => m.name == fieldName);

            fieldInfo ??= fields?.constructorArgs?.FirstOrDefault(m => m.name == fieldName);

            if (fieldInfo != null)
            {
                field = fieldInfo.value;
                return true;
            }
            return false;
        }
    }


    public class CombinedStream : IStream
    {

        public CombinedStream(string alias)
        {
            this.alias = alias;
        }

        /// <summary>
        /// default is ToList, could be :  Count | First | FirstOrDefault | Last | LastOrDefault | TotalCount
        /// </summary>
        public string method { get; set; }


        public string alias { get; protected set; }

        // ExpressionNode_New   new { c = a , d = b }
        public SelectedFields select { get; set; }
        public bool? distinct;
       

        public IStream source;


        public List<StreamToJoin> joins { get; set; }
        public ExpressionNode GetSelectedFields(Type entityType)
        {
            var parameterValue = select?.fields as ExpressionNode;
            if (parameterValue == null && joins?.Any() != true)
            {
                parameterValue = ExpressionNode_RenameableMember.Member(stream: source, entityType);
            }
            return parameterValue;
        }

 

        //  a1.id==b2.id
        public ExpressionNode where { get; set; }



        // ExpressionNode_New     new {  fatherId = left.fatherId, motherId = left.motherId }
        // ExpressionNode_Member  left.fatherId
        public ExpressionNode groupByFields;

        //  left.fatherId >2 && left.Count()>2 && left.Max(m=>m.id) >2
        public ExpressionNode having { get; set; }




        //  a1.id, b2.id
        public List<OrderField> orders { get; set; }


        public int? skip { get; set; }
        public int? take { get; set; }


        public bool isJoinedStream => joins?.Any() == true;
        public bool isGroupedStream => groupByFields!=null;
    }




    public partial class StreamToUpdate : CombinedStream
    {
        public StreamToUpdate(IStream source) : base(source.alias)
        {
            if (source is CombinedStream combinedStream)
            {
                this.select = combinedStream.select;
                this.distinct = combinedStream.distinct;
                this.source = combinedStream.source;
                this.joins = combinedStream.joins;
                this.where = combinedStream.where;
                this.groupByFields = combinedStream.groupByFields;
                this.having = combinedStream.having;
                this.orders = combinedStream.orders;
                this.skip = combinedStream.skip;
                this.take = combinedStream.take;
            }
            else
            {
                base.source = source;
            }
        }

        // ExpressionNode_New   new { name = name + "_" }
        public ExpressionNode_New fieldsToUpdate { get; set; }

    }
}
