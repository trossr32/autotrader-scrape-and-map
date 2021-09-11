using Flurl;

namespace App.Core.Helpers
{
    public static class UrlHelpers
    {
        /// <summary>
        /// Ensures a URL contains the host if the supplied link is relative
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string AsFullUrl(this string url, string domain) => 
            url.Contains(domain) 
                ? url 
                : Url.Combine(domain, url);
    }
}