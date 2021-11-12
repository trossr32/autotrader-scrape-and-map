using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using App.Core.Helpers;
using App.Core.Models;
using App.Core.Models.Configuration;
using App.Core.Services.Configuration;
using Flurl;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace App.Services
{
    public interface IProcessService
    {
        Task Process(bool isTest = false);
    }

    public class ProcessService : IProcessService
    {
        private static ILogger<ProcessService> _logger;
        private static FileSettings _fileSettings;
        private static SearchSettings _searchSettings;

        private static string _searchUrl;

        private static readonly Regex ResultCountRegex = new("(?<ResultCount>\\d+)");
        //private static readonly Regex NoLocationRegex = new Regex("(?<Location>\\.+)[\\d+] miles away");
        private static readonly Regex PriceRegex = new("(?<Price>£\\d+,\\d+)");
        //private static readonly Regex MileageRegex = new("(?<Mileage>\\d+,\\d+)");

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

            _logger?.LogWarning(_fileSettings.RunsDirectory);
            Console.ReadLine();
            return;
            
            SetupRun();

            try
            {
                Results results = await Scrape();
                
                await File.WriteAllTextAsync(Path.Combine(_runDir, "results.json"), JsonConvert.SerializeObject(results, Formatting.Indented));

                await BuildMapScript(results);
            }
            catch (Exception e)
            {
                _logger?.LogError(e, "Scraping failed");
            }
        }
        
        /// <summary>
        /// Scrape Autotrader for advert details
        /// </summary>
        /// <returns></returns>
        private async Task<Results> Scrape()
        {
            using var playwright = await Playwright.CreateAsync();

            await using var browser = await playwright.Firefox.LaunchAsync(new BrowserTypeLaunchOptions { });
            
            var page = await browser.NewPageAsync();
            
            await page.GotoAsync(_searchUrl);

            IElementHandle resultEls = await page.QuerySelectorAsync(".search-form__count");

            int resultsCount = 0;
            int pages = resultEls != null
                ? int.TryParse(ResultCountRegex.Match(await resultEls.InnerTextAsync()).Groups["ResultCount"].Value, out resultsCount)
                    ? (int)Math.Ceiling((decimal)resultsCount / 10)
                    : 1
                : 1;

            var results = new Results {Expected = resultsCount};
            
            _logger?.LogInformation("Expecting {ResultsCount} results", resultsCount);

            // iterate pages of adverts
            for (var i = 1; i <= pages; i++)
            {
                _logger?.LogInformation("Processing page {Page}", i);
                
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
                    var car = new Car(await ad.GetAttributeAsync("href"));
                    
                    _logger?.LogInformation("Processing ad {Url}", car.Url);
                    
                    // open the advert in a new browser page
                    IPage adPage = await browser.NewPageAsync();

                    await adPage.GotoAsync(car.Url.AsFullUrl(_searchSettings.Domain));
                    
                    // wait 1 second to try and improve image quality of snapshot
                    Thread.Sleep(1000);
                    
                    // snapshot entire ad
                    var adSelector = ":is(main > div > div:nth-child(2), main > article > div:nth-child(2))";
                    
                    if (await ElementExists(adSelector, adPage, "ad image", false))
                    {
                        IElementHandle image = await adPage.QuerySelectorAsync(adSelector);
                        
                        if (image == null)
                            _logger.LogError("Failed to retrieve ad image after successfully finding the ad image element");
                        else
                        {
                            car.AdImage = $"{car.Id}_ad.png";
                            var fullSizePath = Path.Combine(_imageDir, $"{car.Id}_ad_full.png");
                            
                            await image.ScreenshotAsync(new ElementHandleScreenshotOptions { Path = fullSizePath });

                            await using var input = File.OpenRead(fullSizePath);

                            using var img = await Image.LoadAsync(input);
                            
                            img.Mutate(r => r.Resize(new ResizeOptions
                            {
                                Size = new Size(img.Width, img.Width),
                                Mode = ResizeMode.Crop,
                                Position = AnchorPositionMode.Top
                            }));
                            
                            await img.SaveAsync(Path.Combine(_imageDir, car.AdImage));
                        }
                    }
                    
                    // snapshot image
                    // var imgSelector = ":is(.gallery__container, section[data-gui='gallery-section'])";
                    //
                    // if (await ElementExists(imgSelector, adPage, "image", false))
                    // {
                    //     IElementHandle image = await page.QuerySelectorAsync(imgSelector);
                    //     
                    //     if (image == null)
                    //         _logger.LogError("Failed to retrieve image after successfully finding the image element");
                    //     else
                    //     {
                    //         car.Image = $"{car.Id}.png";
                    //         
                    //         await image.ScreenshotAsync(new ElementHandleScreenshotOptions { Path = Path.Combine(_imageDir, car.Image) });
                    //     }
                    // }
                    
                    // get price
                    // var priceSelector = "text=/£\\d{2},\\d{3}/i";
                    //
                    // if (await ElementExists(priceSelector, adPage, "price", false))
                    // {
                    //     IElementHandle price = await adPage.QuerySelectorAsync(priceSelector);
                    //     
                    //     if (price == null)
                    //         _logger.LogError("Failed to retrieve price after successfully finding the price element");
                    //     else
                    //         car.Price = PriceRegex.Match(await price.InnerHTMLAsync()).Groups["Price"].Value;
                    // }
                    
                    // get mileage
                    // var mileageSelector = "text=/\\d+,\\d+ miles/i";
                    //
                    // if (await ElementExists(mileageSelector, adPage, "mileage", false))
                    // {
                    //     IElementHandle mileage = await adPage.QuerySelectorAsync(mileageSelector);
                    //     
                    //     if (mileage == null)
                    //         _logger.LogError("Failed to retrieve mileage after successfully finding the mileage element");
                    //     else
                    //         car.Mileage = await mileage.InnerTextAsync();
                    // }

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
                    
                    Thread.Sleep(500);
                    
                    var mapFrameQuery = await adPage.QuerySelectorAsync(mapFrameSelector);

                    if (mapFrameQuery == null)
                    {
                        _logger.LogError("Failed to find the map iframe after successfully finding the map frame element");
                    
                        results.Cars.Add(car);

                        await adPage.CloseAsync();

                        continue;
                    }
                    
                    var mapFrame = await mapFrameQuery.ContentFrameAsync();

                    if (mapFrame == null)
                    {
                        _logger.LogError("Failed to select the map iframe element after successfully finding the map iframe element");
                    
                        results.Cars.Add(car);

                        await adPage.CloseAsync();

                        continue;
                    }
                    
                    var coordsSelector = "div.place-name";

                    if (await ElementExists(coordsSelector, mapFrame, "coordinates"))
                    {
                        IElementHandle coords = await mapFrame.QuerySelectorAsync(coordsSelector);
                        
                        if (coords == null)
                            _logger.LogError("Failed to retrieve coordinates after successfully finding the coordinates element");
                        else
                            car.Coords = await coords.InnerTextAsync();
                    }
                    
                    var addressSelector = "div.address";

                    if (await ElementExists(coordsSelector, mapFrame, "address"))
                    {
                        IElementHandle address = await mapFrame.QuerySelectorAsync(addressSelector);

                        if (address == null)
                            _logger.LogError("Failed to retrieve address after successfully finding the address element");
                        else
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
        private void SetupRun()
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
        private async Task BuildMapScript(Results results) =>
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
        private async Task<bool> ElementExists(string selector, IPage page, string selectorType, bool closePage = true)
        {
            try
            {
                await page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions{Timeout = 3000});

                return true;
            }
            catch (Exception e)
            {
                //await page.PauseAsync();
                
                _logger?.LogError(e, "Unable to find {SelectorType} for advert at {PageUrl}", selectorType, page.Url.AsFullUrl(_searchSettings.Domain));

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
        private async Task<bool> ElementExists(string selector, IFrame frame, string selectorType)
        {
            try
            {
                await frame.WaitForSelectorAsync(selector, new FrameWaitForSelectorOptions{Timeout = 3000});

                return true;
            }
            catch (Exception e)
            {
                _logger?.LogError(e, "Unable to find {SelectorType} for advert at {FrameUrl}", selectorType, frame.Url);

                return false;
            }
        }
    }
}
