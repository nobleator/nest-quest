import { MapState } from './map.js';
import { RadarChartState } from './ui.js';
import { handleSubmit, saveCriteria } from './data.js';
import { addDropdown, initDropdowns, updateSubmitButtonState, showLoading, hideLoading, switchTab } from './ui.js';

const mapState = new MapState();
const radarChartState = new RadarChartState();

document.addEventListener("DOMContentLoaded", async () => {
    updateSubmitButtonState(false);
    await initDropdowns();
    document.getElementById('addButton').addEventListener('click', addDropdown);
    document.getElementById('submitButton').addEventListener('click', async () => {
        showLoading();
        await saveCriteria();
        await handleSubmit(mapState, radarChartState);
        hideLoading();
    });
    document.getElementById('toggle-guide').addEventListener('click', () => {
        const content = document.getElementById('guide-content');
        content.classList.toggle('hidden');
    });
    document.getElementById('mapTabButton').addEventListener('click', () => switchTab('map'));
    document.getElementById('analyticsTabButton').addEventListener('click', () => switchTab('analytics'));
    mapState.initMap();
    await radarChartState.initChart();
});
