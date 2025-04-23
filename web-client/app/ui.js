export function updateSubmitButtonState(enabled=true) {
    const button = document.getElementById('submitButton');
    button.disabled = !enabled;
}

export function addDropdown() {
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

export function removeDropdown(button) {
    const dropdownDiv = button.parentElement;
    dropdownDiv.remove();
    if (document.querySelectorAll('.dropdown').length === 0) {
        updateSubmitButtonState(false);
    }
}

export async function initDropdowns() {
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
            // Scaling back down from m to km
            thresholdInput.value = criterion.tolerance / 1000;
        });
    } catch (error) {
        console.error('Error loading criteria:', error);
    }
}

export function showLoading() {
    document.getElementById('loading-overlay').style.display = 'flex';
}

export function hideLoading() {
    document.getElementById('loading-overlay').style.display = 'none';
}

export function switchTab(tab) {
    const mapTab = document.getElementById('mapTabButton');
    const analyticsTab = document.getElementById('analyticsTabButton');
    const mapContainer = document.getElementById('mapContainer');
    const analyticsContainer = document.getElementById('analyticsContainer');
    if (tab === 'map') {
        analyticsContainer.classList.remove('active');
        mapContainer.classList.add('active');
        mapTab.classList.add('active');
        analyticsTab.classList.remove('active');
    } else if (tab === 'analytics') {
        mapContainer.classList.remove('active');
        analyticsContainer.classList.add('active');
        analyticsTab.classList.add('active');
        mapTab.classList.remove('active');
    }
}

export class RadarChartState {
    constructor() {
        this.chart = null;
        this.labelDict = null;
    }

    async initChart() {
        const response = await fetch('/api/v0/criteria');
        if (!response.ok) throw new Error('Failed to fetch criteria');
        
        const criteria = await response.json();
        this.labelDict = criteria.reduce((acc, item) => {
            acc[item.id] = item.categoryName;
            return acc;
        }, {});

        const data = {
            labels: Object.values(this.labelDict),
            datasets: []
        };
        const config = {
            type: 'radar',
            data: data,
            options: {
              elements: {
                line: {
                  borderWidth: 3
                }
              }
            },
        };
        const ctx = document.getElementById('radar');
        this.chart = new Chart(ctx, config);
    }

    async update(datasets) {
        const response = await fetch('/api/v0/criteria');
        if (!response.ok) throw new Error('Failed to fetch criteria');
        
        const criteria = await response.json();
        this.labelDict = criteria.reduce((acc, item) => {
            acc[item.id] = item.categoryName;
            return acc;
        }, {});
        this.chart.data.labels = Object.values(this.labelDict);
        this.chart.data.datasets = datasets;
        this.chart.update();
    }
}