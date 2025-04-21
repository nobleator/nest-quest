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
        createRadarDiagram();
    }
}

function createRadarDiagram() {
    const data = {
        labels: [
          'Eating',
          'Drinking',
          'Sleeping',
          'Designing',
          'Coding',
          'Cycling',
          'Running'
        ],
        datasets: [{
          label: 'My First Dataset',
          data: [65, 59, 90, 81, 56, 55, 40],
          fill: true,
          backgroundColor: 'rgba(255, 99, 132, 0.2)',
          borderColor: 'rgb(255, 99, 132)',
          pointBackgroundColor: 'rgb(255, 99, 132)',
          pointBorderColor: '#fff',
          pointHoverBackgroundColor: '#fff',
          pointHoverBorderColor: 'rgb(255, 99, 132)'
        }, {
          label: 'My Second Dataset',
          data: [28, 48, 40, 19, 96, 27, 100],
          fill: true,
          backgroundColor: 'rgba(54, 162, 235, 0.2)',
          borderColor: 'rgb(54, 162, 235)',
          pointBackgroundColor: 'rgb(54, 162, 235)',
          pointBorderColor: '#fff',
          pointHoverBackgroundColor: '#fff',
          pointHoverBorderColor: 'rgb(54, 162, 235)'
        }]
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
  new Chart(ctx, config);
}