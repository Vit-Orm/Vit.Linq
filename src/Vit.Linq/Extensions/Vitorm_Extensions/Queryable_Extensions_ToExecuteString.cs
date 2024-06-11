﻿using System;
using System.Linq;
using System.Linq.Expressions;

namespace Vit.Extensions.Linq_Extensions
{

    public static partial class Queryable_Extensions
    {
        /// <summary>
        /// if mysql or sqlserver or sqlite , will get sql string
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string ToExecuteString(this IQueryable source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return source.Provider.Execute<string>(
                Expression.Call(
                    null,
                    new Func<IQueryable, string>(ToExecuteString).Method
                    , source.Expression));
        }
    }
}