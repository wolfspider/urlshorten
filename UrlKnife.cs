
using System.Collections.Generic;

namespace urlshorten
{
    public class UrlKnife
    {
        public string url { get; set; }
        public string normalizedUrl { get; set; }
        public string removedTailOnUrl { get; set; }
        public string protocol { get; set; }
        public string onlyDomain { get; set; }
        public string onlyParams { get; set; }
        public string onlyUri { get; set; }
        public string onlyUriWithParams { get; set; }
        public Dictionary<string, string> onlyParamsJsn { get; set; }
        public string type { get; set; }
        public string port { get; set; }
    }
}