using System;
using CoordinateSharp;

namespace AutotraderScrape.Console.Models
{
    public class Car
    {
        public Guid Id => Guid.NewGuid();
        public string Url { get; set; }
        public string Coords { get; set; }
        public string Location { get; set; }
        public string Price { get; set; }
        public string Image { get; set; }
        public string Mileage { get; set; }
        public string Title { get; set; }
        public bool Success { get; set; }

        public string JsLatLong =>
            Coordinate.TryParse(Coords, out Coordinate c)
                ? $"{{ lat: {c.Latitude.DecimalDegree}, lng: {c.Longitude.DecimalDegree} }}"
                : null;

        public string JsObj => "{" +
                               $"id: '{Id}'," +
                               $"image: '{Image}'," +
                               $"title: '{Title}'," +
                               $"position: {JsLatLong}," +
                               $"link: '{Url}'," +
                               $"price: '{Price}'," +
                               $"mileage: '{Mileage} miles'" +
                               $"location: '{Location}'" +
                               "}";
    }
}