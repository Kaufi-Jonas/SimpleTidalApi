using System;
using System.Collections.Generic;
using System.Web;

namespace TidalLib.Helpers
{
    public static class HttpHelper
    {
        public static Uri BuildQueryString(Uri baseUrl, Dictionary<string, string> data)
        {
            var uriBuilder = new UriBuilder(baseUrl);
            if (data != null)
            {
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);
                foreach (var key in data.Keys)
                    query[key] = data[key];

                uriBuilder.Query = query.ToString();
            }

            return uriBuilder.Uri;
        }
    }
}
