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

let homes = [];
let pointsOfInterest = [];

async function refreshData() {
    let b = map.getBounds();
    let n = b.getNorth();
    let s = b.getSouth();
    let e = b.getEast();
    let w = b.getWest();
    const params = {
        minLat: s,
        maxLat: n,
        minLon: w,
        maxLon: e
    };
    const queryString = new URLSearchParams(params).toString();
    const homesData = await fetch(`/api/v0/homes?${queryString}`).then(res => res.json());
    homesData.forEach((location, index) => {
        const marker = L.marker(location).addTo(map);
        homes.push(marker);
        marker.bindPopup(`Home ${index + 1}`);
    });

    // TODO: parallelize POI fetching by category and link to options and poiIcons

    const attractionsData = await fetch(`/api/v0/attractions?${queryString}`).then(res => res.json());
    attractionsData.forEach(item => {
        const icon = L.divIcon({
            className: 'custom-icon',
            html: `<span style="font-size: 24px;">${poiIcons[item.type]}</span>`,
            iconSize: [30, 30],
            iconAnchor: [15, 30]
        });

        const marker = L.marker(item.location, { icon: icon }).addTo(map);
        pointsOfInterest.push({ marker: marker, type: item.type });
        marker.bindPopup(`${item.type} Attraction`);
    });
}

async function initMap() {
    const map = L.map('map').setView([51.505, -0.09], 13);

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 19,
        attribution: '© OpenStreetMap contributors'
    }).addTo(map);

    map.on('moveend', function() { 
        let b = map.getBounds();
        let n = b.getNorth();
        let s = b.getSouth();
        let e = b.getEast();
        let w = b.getWest();
        alert(`N: ${n} S: ${s} E: ${e} W: ${w}`);
    });

    let b = map.getBounds();
    let n = b.getNorth();
    let s = b.getSouth();
    let e = b.getEast();
    let w = b.getWest();
    const params = {
        minLat: s,
        maxLat: n,
        minLon: w,
        maxLon: e
    };
    const queryString = new URLSearchParams(params).toString();
    const homesData = await fetch(`/api/v0/homes?${queryString}`).then(res => res.json());
    homesData.forEach((location, index) => {
        const marker = L.marker(location).addTo(map);
        homes.push(marker);
        marker.bindPopup(`Home ${index + 1}`);
    });

    const attractionsData = await fetch(`/api/v0/attractions?${queryString}`).then(res => res.json());
    attractionsData.forEach(item => {
        const icon = L.divIcon({
            className: 'custom-icon',
            html: `<span style="font-size: 24px;">${poiIcons[item.type]}</span>`,
            iconSize: [30, 30],
            iconAnchor: [15, 30]
        });

        const marker = L.marker(item.location, { icon: icon }).addTo(map);
        pointsOfInterest.push({ marker: marker, type: item.type });
        marker.bindPopup(`${item.type} Attraction`);
    });
}

function addDropdown() {
    const dropdownContainer = document.getElementById('dropdownContainer');
    const newDropdown = document.createElement('div');
    newDropdown.classList.add('dropdown-container');
    newDropdown.innerHTML = `
        <select class="dropdown">
            <option value="">Select a place type...</option>
            <option value="Unknown">Unknown</option>
            <option value="Library">Library</option>
            <option value="School">School</option>
            <option value="Park">Park</option>
            <option value="BikeTrail">Bike Trail</option>
            <option value="Grocery">Grocery</option>
            <option value="CoffeeShop">Coffee Shop</option>
            <option value="Airport">Airport</option>
            <option value="TrainStation">Train Station</option>
            <option value="BusStation">Bus Station</option>
            <option value="PoliceStation">Police Station</option>
            <option value="FireStation">Fire Station</option>
        </select>
        <input type="number" class="threshold" placeholder="Distance (m)" />
        <button onclick="removeDropdown(this)">Remove</button>
    `;
    dropdownContainer.appendChild(newDropdown);
}

function removeDropdown(button) {
    const dropdownDiv = button.parentElement;
    dropdownDiv.remove();
}

function handleInput() {
    const dropdowns = document.querySelectorAll('.dropdown');
    const thresholds = document.querySelectorAll('.threshold');
    let selectedOptions = Array.from(dropdowns).map((dropdown, index) => ({
        type: dropdown.value,
        threshold: thresholds[index].value
    })).filter(option => option.type && option.threshold);

    const outputDiv = document.getElementById('output');
    outputDiv.innerHTML = `Selected place types and thresholds: ${JSON.stringify(selectedOptions)}`;

    homes.forEach(marker => marker.setOpacity(0.3));

    selectedOptions.forEach(option => {
        homes.forEach(home => {
            const homeLatLng = home.getLatLng();
            pointsOfInterest.forEach(attraction => {
                if (attraction.type === option.type) {
                    const attractionLatLng = attraction.marker.getLatLng();
                    const distance = homeLatLng.distanceTo(attractionLatLng);
                    if (distance <= option.threshold) {
                        home.setOpacity(1);
                    }
                }
            });
        });
    });
};

async function saveCriteria() {
    const sampleCriteria = {
        Criteria: [
            {
                Id: 1,
                Category: 2, 
                Tolerance: 0.5, 
                Unit: 0, 
                Direction: 0 
            },
            { 
                Id: 2,
                Category: 4, 
                Tolerance: 1.0, 
                Unit: 0, 
                Direction: 0 
            }
        ]
    };

    try {
        const response = await fetch('/api/v0/save-criteria', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(sampleCriteria),
        });

        if (response.ok) {
            const result = await response.text();
            alert(`Success: ${result}`);
        } else {
            const error = await response.text();
            alert(`Error: ${error}`);
        }
    } catch (err) {
        console.error('Request failed', err);
        alert('An error occurred while saving criteria.');
    }
};

document.addEventListener("DOMContentLoaded", async () => {
    document.getElementById('saveButton').addEventListener('click', saveCriteria);
    await initMap();
});
