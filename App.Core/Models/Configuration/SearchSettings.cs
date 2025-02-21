namespace App.Core.Models.Configuration;

public class SearchSettings
{
    public string Domain { get; set; } = null!;
    public List<SearchConfig> Searches { get; set; } = [];
}

public class SearchConfig
{
    public string Id { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Url { get; set; } = null!;
}