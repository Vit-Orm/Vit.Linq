using System;
using System.Collections.Generic;
using System.Text;
using Vit.Linq.ComponentModel;

namespace Vit.Linq.Filter.ComponentModel
{
    public class QueryAction
    {
        public IFilterRule filter { get; set; }

        public List<OrderField> orders { get; set; }

        public int? skip { get; set; }
        public int? take { get; set; }


        /// <summary>
        /// default is ToList, could be :  Count | First | FirstOrDefault | Last | LastOrDefault | TotalCount
        /// </summary>
        public string method { get; set; }




        protected IDictionary<string, object> extension { get; set; }


        public void SetValue<T>(string key, T value)
        {
            (extension ??= new Dictionary<string, object>())[key] = value;
        }


        public object GetValue(string key)
        {
            if (extension?.TryGetValue(key, out var value) == true)
            {
                return value;
            }
            return default;
        }

        public bool RemoveKey(string key)
        {
            if (extension?.Remove(key) == true)
            {
                if (extension.Count == 0) extension = null;
                return true;
            }
            return false;
        }
    }
}
