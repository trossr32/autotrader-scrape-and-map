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

  const cars = [{id: 'f8413958575a4379bb5a7017da9ac0f7',adImage: 'f8413958575a4379bb5a7017da9ac0f7_ad.png',link: 'https://www.autotrader.co.uk/car-details/202110158515380?model=F-TYPE&make=JAGUAR&min-engine-power=500&sort=relevance&radius=1500&include-delivery-option=on&price-to=45000&postcode=ss12pe&advertising-location=at_cars&onesearchad=New&onesearchad=Nearly%20New&onesearchad=Used&page=1'},{id: '19e83207bb5c4fcd870fa49e92590a94',adImage: '19e83207bb5c4fcd870fa49e92590a94_ad.png',link: 'https://www.autotrader.co.uk/car-details/202109297931959?model=F-TYPE&make=JAGUAR&min-engine-power=500&sort=relevance&radius=1500&include-delivery-option=on&price-to=45000&postcode=ss12pe&advertising-location=at_cars&onesearchad=New&onesearchad=Nearly%20New&onesearchad=Used&page=1'},{id: '6d39b3dc7b294b6e9b76d0ef5f52be70',adImage: '6d39b3dc7b294b6e9b76d0ef5f52be70_ad.png',link: 'https://www.autotrader.co.uk/car-details/202108216501890?model=F-TYPE&make=JAGUAR&min-engine-power=500&sort=relevance&radius=1500&include-delivery-option=on&price-to=45000&postcode=ss12pe&advertising-location=at_cars&onesearchad=New&onesearchad=Nearly%20New&onesearchad=Used&page=1'},{id: '8b39416c43834c2ea1532a2a40beb86a',adImage: '8b39416c43834c2ea1532a2a40beb86a_ad.png',link: 'https://www.autotrader.co.uk/car-details/202110288999241?model=F-TYPE&make=JAGUAR&min-engine-power=500&sort=relevance&radius=1500&include-delivery-option=on&price-to=45000&postcode=ss12pe&advertising-location=at_cars&onesearchad=New&onesearchad=Nearly%20New&onesearchad=Used&page=1'},{id: '0e724f91e94c4cad9d8779be0154335d',adImage: '0e724f91e94c4cad9d8779be0154335d_ad.png',link: 'https://www.autotrader.co.uk/car-details/202110258876170?model=F-TYPE&make=JAGUAR&min-engine-power=500&sort=relevance&radius=1500&include-delivery-option=on&price-to=45000&postcode=ss12pe&advertising-location=at_cars&onesearchad=New&onesearchad=Nearly%20New&onesearchad=Used&page=1'},{id: '7d2dca8ad5be4b398225d8cfa3b2d4e5',adImage: '7d2dca8ad5be4b398225d8cfa3b2d4e5_ad.png',link: 'https://www.autotrader.co.uk/car-details/202110138456300?model=F-TYPE&make=JAGUAR&min-engine-power=500&sort=relevance&radius=1500&include-delivery-option=on&price-to=45000&postcode=ss12pe&advertising-location=at_cars&onesearchad=New&onesearchad=Nearly%20New&onesearchad=Used&page=1'},{id: 'cbcc0e78719545ac8ac22a80a40e49b2',adImage: 'cbcc0e78719545ac8ac22a80a40e49b2_ad.png',link: 'https://www.autotrader.co.uk/car-details/202110068196190?model=F-TYPE&make=JAGUAR&min-engine-power=500&sort=relevance&radius=1500&include-delivery-option=on&price-to=45000&postcode=ss12pe&advertising-location=at_cars&onesearchad=New&onesearchad=Nearly%20New&onesearchad=Used&page=1'},{id: 'f2c351fe8cb542cb9ebc52c5b72f48da',adImage: 'f2c351fe8cb542cb9ebc52c5b72f48da_ad.png',link: 'https://www.autotrader.co.uk/car-details/202110258870838?model=F-TYPE&make=JAGUAR&min-engine-power=500&sort=relevance&radius=1500&include-delivery-option=on&price-to=45000&postcode=ss12pe&advertising-location=at_cars&onesearchad=New&onesearchad=Nearly%20New&onesearchad=Used&page=1'},{id: '3826950afb154275ae4fd51d296324fc',adImage: '3826950afb154275ae4fd51d296324fc_ad.png',link: 'https://www.autotrader.co.uk/car-details/202111119467152?model=F-TYPE&make=JAGUAR&min-engine-power=500&sort=relevance&radius=1500&include-delivery-option=on&price-to=45000&postcode=ss12pe&advertising-location=at_cars&onesearchad=New&onesearchad=Nearly%20New&onesearchad=Used&page=2'},{id: '638fa2b71f8d4e4f97531ae6ac4e3b37',adImage: '638fa2b71f8d4e4f97531ae6ac4e3b37_ad.png',link: 'https://www.autotrader.co.uk/car-details/202110288985711?model=F-TYPE&make=JAGUAR&min-engine-power=500&sort=relevance&radius=1500&include-delivery-option=on&price-to=45000&postcode=ss12pe&advertising-location=at_cars&onesearchad=New&onesearchad=Nearly%20New&onesearchad=Used&page=2'},{id: '6c5ca12ee3104e20862cd6f5e453ec27',adImage: '6c5ca12ee3104e20862cd6f5e453ec27_ad.png',link: 'https://www.autotrader.co.uk/car-details/202109026902556?model=F-TYPE&make=JAGUAR&min-engine-power=500&sort=relevance&radius=1500&include-delivery-option=on&price-to=45000&postcode=ss12pe&advertising-location=at_cars&onesearchad=New&onesearchad=Nearly%20New&onesearchad=Used&page=2'},{id: 'd937e7c4580c4a29909d1e099e2b0bfa',adImage: 'd937e7c4580c4a29909d1e099e2b0bfa_ad.png',link: 'https://www.autotrader.co.uk/car-details/202110178584283?model=F-TYPE&make=JAGUAR&min-engine-power=500&sort=relevance&radius=1500&include-delivery-option=on&price-to=45000&postcode=ss12pe&advertising-location=at_cars&onesearchad=New&onesearchad=Nearly%20New&onesearchad=Used&page=2'},{id: 'ff76a661ff8c4f2080309de7e38e64dc',adImage: 'ff76a661ff8c4f2080309de7e38e64dc_ad.png',link: 'https://www.autotrader.co.uk/car-details/202108096010726?model=F-TYPE&make=JAGUAR&min-engine-power=500&sort=relevance&radius=1500&include-delivery-option=on&price-to=45000&postcode=ss12pe&advertising-location=at_cars&onesearchad=New&onesearchad=Nearly%20New&onesearchad=Used&page=2'},{id: '513276c78aa444edb4ad41e6a0d5620d',adImage: '513276c78aa444edb4ad41e6a0d5620d_ad.png',link: 'https://www.autotrader.co.uk/car-details/202111059279322?model=F-TYPE&make=JAGUAR&min-engine-power=500&sort=relevance&radius=1500&include-delivery-option=on&price-to=45000&postcode=ss12pe&advertising-location=at_cars&onesearchad=New&onesearchad=Nearly%20New&onesearchad=Used&page=2'},{id: '98047894c7ce4f90bdec9e0f2c63fe9b',adImage: '98047894c7ce4f90bdec9e0f2c63fe9b_ad.png',link: 'https://www.autotrader.co.uk/car-details/202111039192831?model=F-TYPE&make=JAGUAR&min-engine-power=500&sort=relevance&radius=1500&include-delivery-option=on&price-to=45000&postcode=ss12pe&advertising-location=at_cars&onesearchad=New&onesearchad=Nearly%20New&onesearchad=Used&page=2'},{id: '36ceab70acc74cc7af110c5beaa082a3',adImage: '36ceab70acc74cc7af110c5beaa082a3_ad.png',link: 'https://www.autotrader.co.uk/car-details/202109157414869?model=F-TYPE&make=JAGUAR&min-engine-power=500&sort=relevance&radius=1500&include-delivery-option=on&price-to=45000&postcode=ss12pe&advertising-location=at_cars&onesearchad=New&onesearchad=Nearly%20New&onesearchad=Used&page=2'}]
  
  const marker_f8413958575a4379bb5a7017da9ac0f7 = new google.maps.Marker({position: { lat: 51.386, lng: -0.5058611111111111 },map,title: '',});marker_f8413958575a4379bb5a7017da9ac0f7.addListener('click', () => {new google.maps.InfoWindow({content: getImgContentString(cars.find(car => car.id === 'f8413958575a4379bb5a7017da9ac0f7')),}).open({anchor: marker_f8413958575a4379bb5a7017da9ac0f7, map, shouldFocus: false});}); const marker_19e83207bb5c4fcd870fa49e92590a94 = new google.maps.Marker({position: { lat: 51.18486111111111, lng: 0.28952777777777783 },map,title: '',});marker_19e83207bb5c4fcd870fa49e92590a94.addListener('click', () => {new google.maps.InfoWindow({content: getImgContentString(cars.find(car => car.id === '19e83207bb5c4fcd870fa49e92590a94')),}).open({anchor: marker_19e83207bb5c4fcd870fa49e92590a94, map, shouldFocus: false});}); const marker_6d39b3dc7b294b6e9b76d0ef5f52be70 = new google.maps.Marker({position: { lat: 50.86163888888888, lng: 0.5492777777777778 },map,title: '',});marker_6d39b3dc7b294b6e9b76d0ef5f52be70.addListener('click', () => {new google.maps.InfoWindow({content: getImgContentString(cars.find(car => car.id === '6d39b3dc7b294b6e9b76d0ef5f52be70')),}).open({anchor: marker_6d39b3dc7b294b6e9b76d0ef5f52be70, map, shouldFocus: false});}); const marker_8b39416c43834c2ea1532a2a40beb86a = new google.maps.Marker({position: { lat: 52.26797222222222, lng: -0.9800277777777778 },map,title: '',});marker_8b39416c43834c2ea1532a2a40beb86a.addListener('click', () => {new google.maps.InfoWindow({content: getImgContentString(cars.find(car => car.id === '8b39416c43834c2ea1532a2a40beb86a')),}).open({anchor: marker_8b39416c43834c2ea1532a2a40beb86a, map, shouldFocus: false});}); const marker_0e724f91e94c4cad9d8779be0154335d = new google.maps.Marker({position: { lat: 51.3565, lng: -2.123388888888889 },map,title: '',});marker_0e724f91e94c4cad9d8779be0154335d.addListener('click', () => {new google.maps.InfoWindow({content: getImgContentString(cars.find(car => car.id === '0e724f91e94c4cad9d8779be0154335d')),}).open({anchor: marker_0e724f91e94c4cad9d8779be0154335d, map, shouldFocus: false});}); const marker_7d2dca8ad5be4b398225d8cfa3b2d4e5 = new google.maps.Marker({position: { lat: 52.97888888888889, lng: -2.1384722222222226 },map,title: '',});marker_7d2dca8ad5be4b398225d8cfa3b2d4e5.addListener('click', () => {new google.maps.InfoWindow({content: getImgContentString(cars.find(car => car.id === '7d2dca8ad5be4b398225d8cfa3b2d4e5')),}).open({anchor: marker_7d2dca8ad5be4b398225d8cfa3b2d4e5, map, shouldFocus: false});}); const marker_cbcc0e78719545ac8ac22a80a40e49b2 = new google.maps.Marker({position: { lat: 52.73452777777778, lng: -1.5541666666666667 },map,title: '',});marker_cbcc0e78719545ac8ac22a80a40e49b2.addListener('click', () => {new google.maps.InfoWindow({content: getImgContentString(cars.find(car => car.id === 'cbcc0e78719545ac8ac22a80a40e49b2')),}).open({anchor: marker_cbcc0e78719545ac8ac22a80a40e49b2, map, shouldFocus: false});}); const marker_f2c351fe8cb542cb9ebc52c5b72f48da = new google.maps.Marker({position: { lat: 51.466, lng: -3.202 },map,title: '',});marker_f2c351fe8cb542cb9ebc52c5b72f48da.addListener('click', () => {new google.maps.InfoWindow({content: getImgContentString(cars.find(car => car.id === 'f2c351fe8cb542cb9ebc52c5b72f48da')),}).open({anchor: marker_f2c351fe8cb542cb9ebc52c5b72f48da, map, shouldFocus: false});}); const marker_3826950afb154275ae4fd51d296324fc = new google.maps.Marker({position: { lat: 53.372027777777774, lng: -1.39125 },map,title: '',});marker_3826950afb154275ae4fd51d296324fc.addListener('click', () => {new google.maps.InfoWindow({content: getImgContentString(cars.find(car => car.id === '3826950afb154275ae4fd51d296324fc')),}).open({anchor: marker_3826950afb154275ae4fd51d296324fc, map, shouldFocus: false});}); const marker_638fa2b71f8d4e4f97531ae6ac4e3b37 = new google.maps.Marker({position: { lat: 50.85294444444445, lng: -1.4160833333333334 },map,title: '',});marker_638fa2b71f8d4e4f97531ae6ac4e3b37.addListener('click', () => {new google.maps.InfoWindow({content: getImgContentString(cars.find(car => car.id === '638fa2b71f8d4e4f97531ae6ac4e3b37')),}).open({anchor: marker_638fa2b71f8d4e4f97531ae6ac4e3b37, map, shouldFocus: false});}); const marker_6c5ca12ee3104e20862cd6f5e453ec27 = new google.maps.Marker({position: { lat: 53.67997222222222, lng: -1.5079444444444445 },map,title: '',});marker_6c5ca12ee3104e20862cd6f5e453ec27.addListener('click', () => {new google.maps.InfoWindow({content: getImgContentString(cars.find(car => car.id === '6c5ca12ee3104e20862cd6f5e453ec27')),}).open({anchor: marker_6c5ca12ee3104e20862cd6f5e453ec27, map, shouldFocus: false});}); const marker_d937e7c4580c4a29909d1e099e2b0bfa = new google.maps.Marker({position: { lat: 54.9805, lng: -7.2995 },map,title: '',});marker_d937e7c4580c4a29909d1e099e2b0bfa.addListener('click', () => {new google.maps.InfoWindow({content: getImgContentString(cars.find(car => car.id === 'd937e7c4580c4a29909d1e099e2b0bfa')),}).open({anchor: marker_d937e7c4580c4a29909d1e099e2b0bfa, map, shouldFocus: false});}); const marker_ff76a661ff8c4f2080309de7e38e64dc = new google.maps.Marker({position: { lat: 55.028305555555555, lng: -1.5776111111111113 },map,title: '',});marker_ff76a661ff8c4f2080309de7e38e64dc.addListener('click', () => {new google.maps.InfoWindow({content: getImgContentString(cars.find(car => car.id === 'ff76a661ff8c4f2080309de7e38e64dc')),}).open({anchor: marker_ff76a661ff8c4f2080309de7e38e64dc, map, shouldFocus: false});}); const marker_513276c78aa444edb4ad41e6a0d5620d = new google.maps.Marker({position: { lat: 51.13055555555555, lng: -3.0160555555555555 },map,title: '',});marker_513276c78aa444edb4ad41e6a0d5620d.addListener('click', () => {new google.maps.InfoWindow({content: getImgContentString(cars.find(car => car.id === '513276c78aa444edb4ad41e6a0d5620d')),}).open({anchor: marker_513276c78aa444edb4ad41e6a0d5620d, map, shouldFocus: false});}); const marker_98047894c7ce4f90bdec9e0f2c63fe9b = new google.maps.Marker({position: { lat: 54.89452777777778, lng: -2.894916666666667 },map,title: '',});marker_98047894c7ce4f90bdec9e0f2c63fe9b.addListener('click', () => {new google.maps.InfoWindow({content: getImgContentString(cars.find(car => car.id === '98047894c7ce4f90bdec9e0f2c63fe9b')),}).open({anchor: marker_98047894c7ce4f90bdec9e0f2c63fe9b, map, shouldFocus: false});}); const marker_36ceab70acc74cc7af110c5beaa082a3 = new google.maps.Marker({position: { lat: 51.77016666666667, lng: -0.24055555555555558 },map,title: '',});marker_36ceab70acc74cc7af110c5beaa082a3.addListener('click', () => {new google.maps.InfoWindow({content: getImgContentString(cars.find(car => car.id === '36ceab70acc74cc7af110c5beaa082a3')),}).open({anchor: marker_36ceab70acc74cc7af110c5beaa082a3, map, shouldFocus: false});});
}