using System.Collections.Generic;

namespace App.Core.Models.Configuration
{
    public class SearchSettings
    {
        public string Domain { get; set; }
        public List<SearchConfig> Searches { get; set; }
    }

    public class SearchConfig
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
    }
}
