using System.Net.Http;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Vit.Core.Module.Serialization;
using System.Linq;

namespace Vit.Orm.ElasticSearch
{
    public class ElasticSearchClient
    {
        /// <summary>
        /// http://192.168.20.20:9200/dev/info/_bulk
        /// </summary>
        string bulkUrl;


        /// <summary>
        /// es address, example:"http://192.168.20.20:9200"
        /// </summary>
        public string url;


        /// <summary>
        /// es index, example:"dev"
        /// </summary>
        public string index;

        /// <summary>
        /// es type, example:"_doc"
        /// </summary>
        public string type;



        private System.Net.Http.HttpClient httpClient = null;

        public ElasticSearchClient()
        {
            // trust all certificate
            var HttpHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (a, b, c, d) => true
            };
            httpClient = new System.Net.Http.HttpClient(HttpHandler);

            if (string.IsNullOrEmpty(type)) type = "_doc";
            bulkUrl = url + "/" + index + "/" + type + "/_bulk"; 
        }     



  

      
        //public IQueryable<M> Query<M>(string index)
        //{
        //    var request = new HttpRequestMessage(HttpMethod.Post, bulkUrl);

            
        //    httpClient.SendAsync(request); 
        //}
    }
}
