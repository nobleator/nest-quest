import { poiIcons } from './config.js';

async function refreshPoiData(category, mapState) {
    let bounds = mapState.map.getBounds();
    const params = {
        cat: category,
        minLat: bounds.getSouth(),
        maxLat: bounds.getNorth(),
        minLon: bounds.getWest(),
        maxLon: bounds.getEast()
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

        const marker = L.marker(item.location, { icon }).addTo(mapState.poiMarkerLayer);
        marker.bindPopup(`${item.type} POI`);
    }));
}

async function refreshHomeScores(mapState, radarChartState) {
    let bounds = mapState.map.getBounds();
    const params = {
        minLat: bounds.getSouth(),
        maxLat: bounds.getNorth(),
        minLon: bounds.getWest(),
        maxLon: bounds.getEast()
    };
    const queryString = new URLSearchParams(params).toString();
    const homesData = await fetch(`/api/v0/homes?${queryString}`).then(res => res.json());

    // TODO: move to ui.js?
    const matchedHomeList = document.getElementById('matched-homes');
    matchedHomeList.innerHTML = '';
    const unmatchedHomeList = document.getElementById('unmatched-homes');
    unmatchedHomeList.innerHTML = '';
    mapState.homeMarkerLayer.clearLayers();
    let scoreDetails = [];
    const results = await Promise.all(homesData.map(async (home) => {
        const score = await fetch(`/api/v0/score?${new URLSearchParams({
            lat: home.lat,
            lon: home.lon
        }).toString()}`).then(res => res.json());
        home.score = score;

        const detail = await fetch(`/api/v0/score-detail?${new URLSearchParams({
            lat: home.lat,
            lon: home.lon
        }).toString()}`).then(res => res.json());
        const array = Object.keys(radarChartState.labelDict).map(id => detail[id]);
        scoreDetails.push({
            label: home.displayName,
            data: array,
            // fill: true
        });
        return home;
    }));

    await radarChartState.update(scoreDetails);
    
    mapState.heatmapLayer.setData({ max: Math.max(...results.map(h => h.score)), data: results.filter(h => h.score > 0) });

    results.sort((a, b) => a.displayName.localeCompare(b.displayName));
    results.forEach(home => {
        const opacity = home.score === 1 ? 1 : 0.3;
        const marker = L.marker([home.lat, home.lon]);
        marker.setOpacity(opacity);
        marker.addTo(mapState.homeMarkerLayer);
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

function collectCriteria() {
    const dropdownContainers = document.querySelectorAll('.dropdown-container');
    const criteriaList = [];

    dropdownContainers.forEach((container, index) => {
        const dropdown = container.querySelector('.dropdown');
        const thresholdInput = container.querySelector('.threshold');
        const category = parseInt(dropdown.value, 10);
        // Scaling this up to meters to kilometers
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

export async function saveCriteria() {
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

export async function handleSubmit(mapState, radarChartState) {
    mapState.poiMarkerLayer.clearLayers();
    const dropdowns = Array.from(document.querySelectorAll('.dropdown'));
    await Promise.all(dropdowns.map(dropdown => refreshPoiData(dropdown.value, mapState)));
    await refreshHomeScores(mapState, radarChartState);
}