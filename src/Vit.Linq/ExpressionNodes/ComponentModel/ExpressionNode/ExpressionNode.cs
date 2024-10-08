﻿using System.Collections.Generic;

namespace Vit.Linq.ExpressionNodes.ComponentModel
{
    public interface IExpressionNode
    {
        string nodeType { get; }

        /// <summary>
        /// could be: Unary , Binary
        /// </summary>
        string expressionType { get; }

        object GetCodeArg(string key);
        ExpressionNode SetCodeArg(string key, object arg);
    }

    public partial class ExpressionNode : IExpressionNode
    {
        public string nodeType { get; set; }


        /// <summary>
        /// could be: Unary , Binary
        /// </summary>
        public string expressionType { get; set; }

        protected Dictionary<string, object> codeArg;
        public object GetCodeArg(string key) => codeArg?.TryGetValue(key, out var value) == true ? value : null;
        public Dictionary<string, object> GetCodeArg() => codeArg;
        public ExpressionNode SetCodeArg(string key, object arg)
        {
            codeArg ??= new();
            codeArg[key] = arg;
            return this;
        }
    }
}
