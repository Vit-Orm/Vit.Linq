using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Vit.Linq.ExpressionNodes.ComponentModel
{
    public class ExpressionNodeCloner
    {
        public virtual Func<ExpressionNode, (bool success, ExpressionNode dest)> clone { get; set; }
        public virtual ExpressionNode Clone(ExpressionNode node)
        {
            if (node == null) return null;

            if (clone != null)
            {
                var (success, dest) = clone(node);
                if (success) return dest;
            }
            return CloneChildren(node, new ExpressionNode());
        }

        static readonly PropertyInfo[] properties = typeof(ExpressionNode).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        public virtual ExpressionNode CloneChildren(ExpressionNode source, ExpressionNode destination)
        {
            foreach (var p in properties)
            {
                if (p.CanRead && p.CanWrite)
                {
                    var value = p.GetValue(source);
                    if (value == null) continue;
                    if (value is ExpressionNode child)
                    {
                        value = Clone(child);
                    }
                    else if (value is ExpressionNode[] childrenArray)
                    {
                        value = childrenArray.Select(child => Clone(child)).ToArray();
                    }
                    else if (value is List<ExpressionNode> childrenList)
                    {
                        value = childrenList.Select(child => Clone(child)).ToList();
                    }
                    else if (value is List<MemberBind> members)
                    {
                        value = members.Select(member => new MemberBind { name = member.name, value = Clone(member.value) }).ToList();
                    }
                    p.SetValue(destination, value);
                }
            }
            var codeArg = source.GetCodeArg();
            if (codeArg != null)
            {
                foreach (var kv in codeArg)
                    destination.SetCodeArg(kv.Key, kv.Value);
            }
            return destination;
        }
    }
}
