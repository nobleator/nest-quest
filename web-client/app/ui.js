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

function displaySuggestedAddresses(addresses) {
    const suggestionsTableBody = document.getElementById('suggested-addresses').querySelector('tbody');
    suggestionsTableBody.innerHTML = '';
  
    addresses.forEach(place => {
        const row = document.createElement('tr');
    
        const addressCell = document.createElement('td');
        addressCell.textContent = place.address;
        row.appendChild(addressCell);
    
        const actionCell = document.createElement('td');
        const addButton = document.createElement('button');
        addButton.textContent = '+';
        addButton.addEventListener('click', () => {
            savePlaceToBackend(place);
            row.remove();
        });
        actionCell.appendChild(addButton);
        row.appendChild(actionCell);
    
        suggestionsTableBody.appendChild(row);
    });
}

async function savePlaceToBackend(place) {
    try {
        const response = await fetch(`/api/v0/places`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(place)
        });

        if (!response.ok) throw new Error('Failed to save place.');

        const savedPlace = await response.json();
        addRowToTable(savedPlace);
    } catch (error) {
        console.error(error);
        alert('Error saving place. Please try again.');
    }
}
  
export function addRowToTable(place) {
    const tableBody = document.getElementById('savedPlacesTable').querySelector('tbody');
  
    const row = document.createElement('tr');
    row.dataset.id = place.id;
  
    const addressCell = document.createElement('td');
    addressCell.textContent = place.address;
    row.appendChild(addressCell);
  
    const actionCell = document.createElement('td');
    const deleteButton = document.createElement('button');
    deleteButton.innerHTML = '&times;';
    deleteButton.addEventListener('click', () => deleteRow(row, place.id));
    actionCell.appendChild(deleteButton);
    row.appendChild(actionCell);
  
    tableBody.appendChild(row);
}
  
async function deleteRow(row, id) {
    try {
        const response = await fetch(`/api/v0/places/${id}`, { method: 'DELETE' });
        if (!response.ok) throw new Error('Failed to delete place.');

        row.remove();
    } catch (error) {
        console.error(error);
        alert('Error deleting place. Please try again.');
    }
}

export async function handleSearch() {
    const addressInput = document.getElementById('addressInput').value;
    if (!addressInput.trim()) {
        alert('Please enter a valid address.');
        return;
    }
    
    try {
        const response = await fetch(`/api/v0/geocode?address=${encodeURIComponent(addressInput)}`);
        if (!response.ok) throw new Error('Failed to fetch geocode data.');
    
        const data = await response.json();
        displaySuggestedAddresses(data);
        openModal();
    } catch (error) {
        console.error(error);
        alert('Error fetching data. Please try again.');
    }
}

function openModal() {
    document.getElementById('suggested-modal').style.display = 'block';
}

export function closeModal() {
    document.getElementById('suggested-modal').style.display = 'none';
}