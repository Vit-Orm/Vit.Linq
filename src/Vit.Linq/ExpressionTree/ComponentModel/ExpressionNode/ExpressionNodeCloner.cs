using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Vit.Linq.ExpressionTree.ComponentModel 
{
    public class ExpressionNodeCloner
    {
        public virtual Func<ExpressionNode, (bool success, ExpressionNode dest)> clone { get; set; }
        public virtual ExpressionNode Clone(ExpressionNode node)
        {
            if (clone != null)
            {
                var result = clone(node);
                if (result.success) return result.dest;
            }
            return CloneChildren(node, new ExpressionNode());
        }

        public virtual ExpressionNode CloneChildren(ExpressionNode source, ExpressionNode dest)
        {
            foreach (var p in typeof(ExpressionNode).GetProperties())
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
                    p.SetValue(dest, value);
                }
            }
            var codeArg = source.GetCodeArg();
            if (codeArg != null)
            {
                foreach (var kv in codeArg)
                    dest.SetCodeArg(kv.Key, kv.Value);
            }
            return dest;
        }
    }
}
