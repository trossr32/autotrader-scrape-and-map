using System.Collections.Generic;
using System.Linq;

namespace App.Core.Models
{
    public class Results
    {
        public int Expected { get; set; }
        public List<string> Pages { get; set; } = new();
        public List<Car> Cars { get; set; } = new();

        public IEnumerable<Car> SuccessfulCars => Cars.Where(c => c.Success && !string.IsNullOrEmpty(c.Coords));

        public string JsCarsArray => $"const cars = [{SuccessfulCars.Select(car => car.JsObj).Aggregate((a, b) => $"{a},{b}")}]";

        public string JsMarkers =>
            SuccessfulCars.Select(car =>
                    $"const marker_{car.Id} = new google.maps.Marker({{" +
                    $"position: {car.JsLatLong}," +
                    "map," +
                    $"title: '{car.Title}'," +
                    "});" +

                    $"marker_{car.Id}.addListener('click', () => {{" +
                    "new google.maps.InfoWindow({" +
                    //$"content: getContentString(cars.find(car => car.id === '{car.Id}'))," +
                    $"content: getImgContentString(cars.find(car => car.id === '{car.Id}'))," +
                    "}).open({" +
                    $"anchor: marker_{car.Id}, map, shouldFocus: false" +
                    "});" +
                    "});")
                .Aggregate((a, b) => $"{a} {b}");
    }
}