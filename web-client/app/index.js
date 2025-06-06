import { MapState } from './map.js';
import { RadarChartState } from './ui.js';
import { handleSubmit, saveCriteria } from './data.js';
import { addDropdown, initDropdowns, updateSubmitButtonState, showLoading, hideLoading, switchTab, addRowToTable, handleSearch, closeModal } from './ui.js';

const mapState = new MapState();
const radarChartState = new RadarChartState();

document.addEventListener("DOMContentLoaded", async () => {
    try {
        const response = await fetch(`/api/v0/health`);
        if (!response.ok) throw new Error('Failed to fetch healthcheck.');
        const settings = await response.json();
        document.getElementById('app-version').innerText = settings.version;
    } catch (error) {
        console.error(error);
    }
    updateSubmitButtonState(false);
    await initDropdowns();
    document.getElementById('add-button').addEventListener('click', addDropdown);
    document.getElementById('submit-button').addEventListener('click', async () => {
        showLoading();
        await saveCriteria();
        await handleSubmit(mapState, radarChartState);
        hideLoading();
    });
    document.getElementById('toggle-guide').addEventListener('click', () => {
        const content = document.getElementById('guide-content');
        content.classList.toggle('hidden');
    });
    document.getElementById('map-tab-button').addEventListener('click', () => switchTab('map'));
    document.getElementById('analytics-tab-button').addEventListener('click', () => switchTab('analytics'));

    try {
        const response = await fetch(`/api/v0/places`);
        if (!response.ok) throw new Error('Failed to fetch saved places.');
        const savedPlaces = await response.json();
        savedPlaces.forEach(place => addRowToTable(place));
    } catch (error) {
        console.error(error);
        alert('Error loading saved places. Please try again.');
    }
    document.getElementById('search-button').addEventListener('click', handleSearch);
    document.getElementById('close-modal').addEventListener('click', closeModal);

    mapState.initMap();
    await radarChartState.initChart();
});
