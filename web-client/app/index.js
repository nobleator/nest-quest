import { MapState } from './map.js';
import { handleSubmit, saveCriteria } from './data.js';
import { addDropdown, initDropdowns, updateSubmitButtonState, showLoading, hideLoading } from './ui.js';

const mapState = new MapState();

document.addEventListener("DOMContentLoaded", async () => {
    updateSubmitButtonState(false);
    await initDropdowns();
    document.getElementById('addButton').addEventListener('click', addDropdown);
    document.getElementById('submitButton').addEventListener('click', async () => {
        showLoading();
        await saveCriteria();
        await handleSubmit(mapState);
        hideLoading();
    });
    document.getElementById('toggle-guide').addEventListener('click', () => {
        const content = document.getElementById('guide-content');
        content.classList.toggle('hidden');
    });
    mapState.initMap();
});
