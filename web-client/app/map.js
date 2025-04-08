import { heatmapConfig } from './config.js';
import { handleSubmit } from './data.js';

export class MapState {
    constructor() {
        this.map = null;
        this.homeMarkerLayer = null;
        this.poiMarkerLayer = null;
        this.heatmapLayer = null;
    }

    initMap() {
        this.map = L.map('map').setView([51.505, -0.09], 13);
        this.homeMarkerLayer = L.layerGroup().addTo(this.map);
        this.poiMarkerLayer = L.layerGroup().addTo(this.map);
        this.heatmapLayer = new HeatmapOverlay(heatmapConfig);
        this.heatmapLayer.addTo(this.map);

        L.tileLayer('https://tiles.stadiamaps.com/tiles/alidade_smooth/{z}/{x}/{y}{r}.{ext}', {
            minZoom: 0,
            maxZoom: 19,
            attribution: '&copy; <a href="https://www.stadiamaps.com/" target="_blank">Stadia Maps</a> &copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors',
            ext: 'png'
        }).addTo(this.map);
        
        // Note: arrow function is required to bind `this` to the instance of the MapState class
        this.map.on('moveend', () => handleSubmit(this));
        var pinIcon = L.divIcon({
            className: 'pin-marker',
            html: '<span style="font-size: 32px;">📍</span>',
            iconSize: [30, 30],
            iconAnchor: [15, 30],
            popupAnchor: [0, -30]
        });
        var pin = L.marker([51.505, -0.09], {
            draggable: true,
            icon: pinIcon
        }).addTo(this.map);
        updatePinPopup(pin);
    
        pin.on('dragend', function() {
            updatePinPopup(pin);
        });
    }
}

async function updatePinPopup(marker) {
    var latlng = marker.getLatLng();
    var score = await fetch(`/api/v0/score?${new URLSearchParams({
        lat: latlng.lat,
        lon: latlng.lng
    }).toString()}`).then(res => res.json());
    marker.bindPopup(`Score: ${score}`).openPopup();
}
