function getContentString(car) {
  return `<div class="marker-container">` +
      `<div class="marker-image">` +
      `<img src="images/${car.image}">` +
      "</div>" +
      `<div class="marker-content">` +
      `<h3>${car.title}</h3>` +
      `<p>${car.price}</p>` +
      `<p>${car.mileage}</p>` +
      `<p>${car.location}</p>` +
      `<a target="_blank" href="${car.link}">Autotrader link</a>` +
      "</div>" +
      "</div>";
}

function getImgContentString(car) {
  return `<div class="marker-img-container">` +
      `<img src="images/${car.adImage}">` +
      `<br /><br />` +
      `<a target="_blank" href="${car.link}">Autotrader link</a>` +
      "</div>";
}

function initMap() {
  const map = new google.maps.Map(document.getElementById("map"), {
    zoom: 7,
    center: { lat: 53.00853, lng: -0.12574 },
  });

  //CARS_ARRAY_PLACEHOLDER
  
  //MARKERS_ARRAY_PLACEHOLDER
}