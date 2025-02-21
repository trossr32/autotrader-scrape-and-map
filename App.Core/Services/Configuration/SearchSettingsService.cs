using App.Core.Models.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace App.Core.Services.Configuration;

public interface ISearchSettingsService
{
    SearchSettings GetSettings();
}

public class SearchSettingsService : ISearchSettingsService
{
    private readonly ILogger<SearchSettingsService> _logger;
    private readonly SearchSettings _searchSettings;

    public SearchSettingsService(ILogger<SearchSettingsService> logger, IOptions<SearchSettings> searchSettings)
    {
        _logger = logger;
        _searchSettings = searchSettings.Value;
    }

    public SearchSettings GetSettings() => _searchSettings;
}