const poiIcons = {
    'Unknown': '?',
    'Library': '📚',
    'School': '🏫',
    'Park': '🏞️',
    'BikeTrail': '🚴',
    'Grocery': '🛒',
    'CoffeeShop': '☕',
    'Airport': '✈️',
    'TrainStation': '🚉',
    'BusStation': '🚏',
    'PoliceStation': '👮‍♂️',
    'FireStation': '🚒'
};

const heatmapConfig = {
    // radius should be small ONLY if scaleRadius is true (or small radius is intended)
    // if scaleRadius is false it will be the constant radius used in pixels
    "radius": 60,
    "maxOpacity": .8,
    // scales the radius based on map zoom
    "scaleRadius": false,
    // if set to false the heatmap uses the global maximum for colorization
    // if activated: uses the data maximum within the current map boundaries
    //   (there will always be a red spot with useLocalExtremas true)
    "useLocalExtrema": false,
    // which field name in your data represents the latitude - default "lat"
    latField: 'lat',
    // which field name in your data represents the longitude - default "lng"
    lngField: 'lon',
    // which field name in your data represents the data value - default "value"
    valueField: 'score',
    // blur: 15,
    // max: 1,
};

let homes = [];
let map;
let homeMarkerLayer;
let poiMarkerLayer;
let heatmapLayer;
let isSubmitEnabled;

async function refreshPoiData(category) {
    let b = map.getBounds();
    let n = b.getNorth();
    let s = b.getSouth();
    let e = b.getEast();
    let w = b.getWest();
    const params = {
        cat: category,
        minLat: s,
        maxLat: n,
        minLon: w,
        maxLon: e
    };
    const queryString = new URLSearchParams(params).toString();
    
    const poiData = await fetch(`/api/v0/poi?${queryString}`).then(res => res.json());
    await Promise.all(poiData.map(item => {
        const icon = L.divIcon({
            className: 'custom-icon',
            html: `<span style="font-size: 24px;">${poiIcons[item.type]}</span>`,
            iconSize: [30, 30],
            iconAnchor: [15, 30]
        });

        const marker = L.marker(item.location, { icon: icon }).addTo(poiMarkerLayer);
        marker.bindPopup(`${item.type} POI`);
    }));
}

async function refreshHomeScores() {
    const homesData = await fetch(`/api/v0/homes?${new URLSearchParams({
        minLat: map.getBounds().getSouth(),
        maxLat: map.getBounds().getNorth(),
        minLon: map.getBounds().getWest(),
        maxLon: map.getBounds().getEast()
    }).toString()}`).then(res => res.json());

    const matchedHomeList = document.getElementById('matchedHomes');
    matchedHomeList.innerHTML = '';
    const unmatchedHomeList = document.getElementById('unmatchedHomes');
    unmatchedHomeList.innerHTML = '';
    homeMarkerLayer.clearLayers();
    const results = await Promise.all(homesData.map(async (home) => {
        const score = await fetch(`/api/v0/score?${new URLSearchParams({
            lat: home.lat,
            lon: home.lon
        }).toString()}`).then(res => res.json());
        home.score = score;
        return home;
    }));
    
    heatmapLayer.setData({ max: Math.max(...results.map(h => h.score)), data: results.filter(h => h.score > 0) });

    results.sort((a, b) => a.displayName.localeCompare(b.displayName));
    results.forEach(home => {
        const opacity = home.score === 1 ? 1 : 0.3;
        const marker = L.marker([home.lat, home.lon]);
        marker.setOpacity(opacity);
        marker.addTo(homeMarkerLayer);
        homes.push(marker);
        marker.bindPopup(home.displayName);
        const li = document.createElement('li');
        li.textContent = home.displayName;
        if (home.score === 1) {
            matchedHomeList.appendChild(li);
        } else {
            unmatchedHomeList.appendChild(li);
        }
    });
}

async function updatePinPopup(marker) {
    var latlng = marker.getLatLng();
    var score = await fetch(`/api/v0/score?${new URLSearchParams({
        lat: latlng.lat,
        lon: latlng.lng
    }).toString()}`).then(res => res.json());
    marker.bindPopup(`Score: ${score}`).openPopup();
}


async function initMap() {
    map = L.map('map').setView([51.505, -0.09], 13);
    homeMarkerLayer = L.layerGroup().addTo(map);
    poiMarkerLayer = L.layerGroup().addTo(map);
    heatmapLayer = new HeatmapOverlay(heatmapConfig);
    heatmapLayer.addTo(map);

    L.tileLayer('https://tiles.stadiamaps.com/tiles/alidade_smooth/{z}/{x}/{y}{r}.{ext}', {
        minZoom: 0,
        maxZoom: 19,
        attribution: '&copy; <a href="https://www.stadiamaps.com/" target="_blank">Stadia Maps</a> &copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors',
        ext: 'png'
    }).addTo(map);

    map.on('moveend', handleSubmit);

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
    }).addTo(map);
    updatePinPopup(pin);

    pin.on('dragend', function() {
        updatePinPopup(pin);
    });
}

function updateSubmitButtonState(enabled=true) {
    const button = document.getElementById('submitButton');
    button.disabled = !enabled;
}

function addDropdown() {
    const dropdownContainer = document.getElementById('dropdownContainer');
    const newDropdown = document.createElement('div');
    newDropdown.classList.add('dropdown-container');
    newDropdown.innerHTML = `
        <select class="dropdown">
            <option value="">Select a place type...</option>
            <option value="0">Unknown</option>
            <option value="1">Library</option>
            <option value="2">School</option>
            <option value="3">Park</option>
            <option value="4">Bike Trail</option>
            <option value="5">Grocery</option>
            <option value="6">Coffee Shop</option>
            <option value="7">Airport</option>
            <option value="8">Train Station</option>
            <option value="9">Bus Station</option>
            <option value="10">Police Station</option>
            <option value="11">Fire Station</option>
        </select>
        <label><input type="number" class="threshold" /> (km)</label>
        <button onclick="removeDropdown(this)">Remove</button>
    `;
    dropdownContainer.appendChild(newDropdown);
    updateSubmitButtonState(true);
    return newDropdown;
}

function removeDropdown(button) {
    const dropdownDiv = button.parentElement;
    dropdownDiv.remove();
    if (document.querySelectorAll('.dropdown').length === 0) {
        updateSubmitButtonState(false);
    }
}

async function initDropdowns() {
    try {
        const response = await fetch('/api/v0/criteria');
        if (!response.ok) throw new Error('Failed to fetch criteria');
        
        const criteria = await response.json();
        const dropdownContainer = document.getElementById('dropdownContainer');
        dropdownContainer.innerHTML = '';

        criteria.forEach(criterion => {
            const newDropdown = addDropdown();
            const dropdown = newDropdown.querySelector('.dropdown');
            const thresholdInput = newDropdown.querySelector('.threshold');
            dropdown.value = criterion.category;
            // Scaling back down
            thresholdInput.value = criterion.tolerance / 1000;
        });
    } catch (error) {
        console.error('Error loading criteria:', error);
    }
}

function collectCriteria() {
    const dropdownContainers = document.querySelectorAll('.dropdown-container');
    const criteriaList = [];

    dropdownContainers.forEach((container, index) => {
        const dropdown = container.querySelector('.dropdown');
        const thresholdInput = container.querySelector('.threshold');
        const category = parseInt(dropdown.value, 10);
        // Scaling this down from kilometers to meters
        const tolerance = parseFloat(thresholdInput.value) * 1000;

        if (!isNaN(category) && !isNaN(tolerance)) {
            criteriaList.push({
                Id: index + 1,
                Category: category,
                Tolerance: tolerance,
                Unit: 0,
                Direction: 0
            });
        }
    });

    return { Criteria: criteriaList };
}

async function saveCriteria() {
    var criteria = collectCriteria();

    try {
        const response = await fetch('/api/v0/criteria', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(criteria),
        });

        if (response.ok) {
            const result = await response.text();
            console.log(`Save result: ${result}`);
        } else {
            const error = await response.text();
            console.error(`Error: ${error}`);
        }
    } catch (err) {
        console.error('Request failed', err);
    }
};

function showLoading() {
    document.getElementById('loading-overlay').style.display = 'flex';
}

function hideLoading() {
    document.getElementById('loading-overlay').style.display = 'none';
}

async function handleSubmit() {
    poiMarkerLayer.clearLayers();
    const dropdowns = Array.from(document.querySelectorAll('.dropdown'));
    await Promise.all(dropdowns.map(dropdown => refreshPoiData(dropdown.value)));
    await refreshHomeScores();
}

document.addEventListener("DOMContentLoaded", async () => {
    updateSubmitButtonState(false);
    await initDropdowns();
    document.getElementById('submitButton').addEventListener('click', async () => {
        showLoading();
        await saveCriteria();
        await handleSubmit();
        hideLoading();
    });
    document.getElementById('toggle-guide').addEventListener('click', () => {
        const content = document.getElementById('guide-content');
        content.classList.toggle('hidden');
    });
    await initMap();
});