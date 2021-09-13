using System;
using App.Core.Helpers;
using CoordinateSharp;

namespace App.Core.Models
{
    public class Car
    {
        public Car()
        {
            Id = Guid.NewGuid().ToString().Replace("-", "");
        }
        
        public Car(string url)
        {
            Id = Guid.NewGuid().ToString().Replace("-", "");
            Url = url;
        }
        
        public string Id { get; set; }
        public string Url { get; set; }
        public string Coords { get; set; }
        public string Location { get; set; }
        public string Price { get; set; }
        public string Image { get; set; }
        public string AdImage { get; set; }
        public string Mileage { get; set; }
        public string Title { get; set; }
        public bool Success { get; set; }

        public string JsLatLong =>
            Coordinate.TryParse(Coords, out Coordinate c)
                ? $"{{ lat: {c.Latitude.DecimalDegree}, lng: {c.Longitude.DecimalDegree} }}"
                : null;

        public string JsObj => "{" +
                               $"id: '{Id}'," +
                               // $"image: '{Image}'," +
                               $"adImage: '{AdImage}'," +
                               // $"title: '{Title}'," +
                               // $"position: {JsLatLong}," +
                               $"link: '{Url.AsFullUrl("https://www.autotrader.co.uk")}'" +
                               // $"price: '{Price}'," +
                               // $"mileage: '{Mileage} miles'," +
                               // $"location: '{Location}'" +
                               "}";
    }
}