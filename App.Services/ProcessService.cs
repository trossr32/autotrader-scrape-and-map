using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using App.Core.Helpers;
using App.Core.Models.Configuration;
using App.Core.Services.Configuration;
using AutotraderScrape.Console.Models;
using Flurl;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using Newtonsoft.Json;

namespace App.Services
{
    public interface IProcessService
    {
        Task Process(bool isTest = false);
    }

    public class ProcessService : IProcessService
    {
        private readonly ILogger<ProcessService> _logger;
        private static FileSettings _fileSettings;
        private static SearchSettings _searchSettings;

        private static string _searchUrl;

        private static readonly Regex ResultCountRegex = new("(?<ResultCount>\\d+)");
        //private static readonly Regex NoLocationRegex = new Regex("(?<Location>\\.+)[\\d+] miles away");

        private static readonly string AssetsDir = Path.Combine(Environment.CurrentDirectory, "Assets");
        private static string _runDir;
        private static string _imageDir;
        private static string _indexJsFile;

        public ProcessService(ILogger<ProcessService> logger, IFileSettingsService fileSettingsSvc, ISearchSettingsService searchSettingsSvc)
        {
            _logger = logger;

            _fileSettings = fileSettingsSvc.GetSettings();

            _searchSettings = searchSettingsSvc.GetSettings();
            
            _runDir = Path.Combine(_fileSettings.RunsDirectory, $"{DateTime.Now:yyyyMMdd HHmmss}");
            _imageDir = Path.Combine(_runDir, "images");
            _indexJsFile = Path.Combine(_runDir, "index.js");
            
            _searchUrl = Url.Combine(_searchSettings.Domain, _searchSettings.SearchUrl);
        }

        public async Task Process(bool isTest = false)
        {
            if (isTest)
                _logger?.LogInformation("Testing!");
            
            SetupRun();

            var results = await Scrape();

            Console.Write(JsonConvert.SerializeObject(results, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore }));

            await BuildMapScript(results);
        }
        
        /// <summary>
        /// Scrape Autotrader for advert details
        /// </summary>
        /// <returns></returns>
        private static async Task<Results> Scrape()
        {
            using var playwright = await Playwright.CreateAsync();

            await using var browser = await playwright.Firefox.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false, SlowMo = 50 });
            
            var page = await browser.NewPageAsync();
            
            await page.GotoAsync(_searchUrl);

            var resultEls = await page.QuerySelectorAsync(".search-form__count");

            var pages = int.TryParse(ResultCountRegex.Match(await resultEls.InnerTextAsync()).Groups["ResultCount"].Value, out int resultsCount)
                ? (int)Math.Ceiling((decimal)resultsCount / 10)
                : 1;

            var results = new Results {Expected = resultsCount};

            // iterate pages of adverts
            for (var i = 1; i <= pages; i++)
            {
                // if this isn't the first page then append the page number and navigate
                if (i > 1)
                {
                    var navPage = _searchUrl.SetQueryParam("page", i);
                    
                    results.Pages.Add(navPage);
                    
                    await page.GotoAsync(navPage);
                }
                else
                    results.Pages.Add(_searchUrl);

                // get adverts on page
                var ads = await page.QuerySelectorAllAsync("article.product-card[data-standout-type=''] > a");
                
                // iterate all adverts
                foreach (var ad in ads)
                {
                    // build a car object
                    var car = new Car
                    {
                        Url = await ad.GetAttributeAsync("href")
                    };
                    
                    // open the advert in a new browser page
                    var adPage = await browser.NewPageAsync();

                    await adPage.GotoAsync(car.Url.AsFullUrl(_searchSettings.Domain));
                    
                    // todo - get price
                    // todo - snapshot image
                    // todo - get mileage

                    // try to find a location link
                    var locBtnSelector = ":is(button[data-gui-test='dealerLocationLink'], button.seller-location__toggle)";

                    if (!await ElementExists(locBtnSelector, adPage, "location"))
                    {
                        results.Cars.Add(car);
                        
                        continue;
                    }

                    await adPage.ClickAsync(locBtnSelector);
                    
                    var mapFrameSelector = "iframe[src^='https://www.google.com/maps/embed']";

                    if (!await ElementExists(mapFrameSelector, adPage, "map frame", false))
                    {
                        results.Cars.Add(car);
                        
                        continue;
                    }
                    
                    var mapFrameQuery = await adPage.QuerySelectorAsync(mapFrameSelector);
                    var mapFrame = await mapFrameQuery.ContentFrameAsync();
                    
                    var coordsSelector = "div.place-name";

                    if (await ElementExists(coordsSelector, mapFrame, "coordinates"))
                    {
                        var coords = await mapFrame.QuerySelectorAsync(coordsSelector);

                        car.Coords = await coords.InnerTextAsync();
                    }
                    
                    var addressSelector = "div.address";

                    if (await ElementExists(coordsSelector, mapFrame, "address"))
                    {
                        var address = await mapFrame.QuerySelectorAsync(addressSelector);

                        car.Location = await address.InnerTextAsync();
                    }

                    car.Success = true;
                    
                    results.Cars.Add(car);

                    await adPage.CloseAsync();
                }
            }

            return results;
        }

        /// <summary>
        /// Set up the run directory
        /// </summary>
        private static void SetupRun()
        {
            if (!Directory.Exists(_fileSettings.RunsDirectory))
                Directory.CreateDirectory(_fileSettings.RunsDirectory);

            Directory.CreateDirectory(_runDir);
            
            foreach (var file in Directory.GetFiles(AssetsDir))
            {
                File.Copy(file, Path.Combine(_runDir, Path.GetFileName(file)));
            }

            Directory.CreateDirectory(_imageDir);
        }

        /// <summary>
        /// Build Google map script
        /// </summary>
        /// <param name="results"></param>
        private static async Task BuildMapScript(Results results) =>
            await File.WriteAllTextAsync(_indexJsFile, (await File.ReadAllTextAsync(_indexJsFile))
                .Replace("//CARS_ARRAY_PLACEHOLDER", results.JsCarsArray)
                .Replace("//MARKERS_ARRAY_PLACEHOLDER", results.JsMarkers));
        
        /// <summary>
        /// Checks that an element exists in a page
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="page"></param>
        /// <param name="selectorType"></param>
        /// <param name="closePage"></param>
        /// <returns></returns>
        private static async Task<bool> ElementExists(string selector, IPage page, string selectorType, bool closePage = true)
        {
            try
            {
                await page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions{Timeout = 3000});

                return true;
            }
            catch (Exception)
            {
                // couldn't find a location
                await Console.Error.WriteLineAsync($"Unable to find {selectorType} for advert at {page.Url.AsFullUrl(_searchSettings.Domain)}");

                if (closePage)
                    await page.CloseAsync();

                return false;
            }
        }

        /// <summary>
        /// Checks that an element exists in a frame
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="frame"></param>
        /// <param name="selectorType"></param>
        /// <returns></returns>
        private static async Task<bool> ElementExists(string selector, IFrame frame, string selectorType)
        {
            try
            {
                await frame.WaitForSelectorAsync(selector, new FrameWaitForSelectorOptions{Timeout = 3000});

                return true;
            }
            catch (Exception)
            {
                // couldn't find a location
                await Console.Error.WriteLineAsync($"Unable to find {selectorType} for advert at {frame.Url}");

                return false;
            }
        }
    }
}
