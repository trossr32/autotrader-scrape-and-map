using App.Core.Models.Configuration;

namespace App.Core.Helpers;

public static class StringHelpers
{
    /// <summary>
    /// Json array for search configs
    /// </summary>
    /// <param name="searchSettings"></param>
    /// <returns></returns>
    public static string SearchesJsonArray(this SearchSettings searchSettings) =>
        $"const searches = [{searchSettings.Searches.ToList().Select(SearchSettingJson).Aggregate((a, b) => $"{a},{b}")}]";

    /// <summary>
    /// Create a json search config item
    /// </summary>
    /// <param name="searchConfig"></param>
    /// <returns></returns>
    private static string SearchSettingJson(this SearchConfig searchConfig) =>
        $"{{id: '{searchConfig.Id}', description: '{searchConfig.Description}'}}";
}