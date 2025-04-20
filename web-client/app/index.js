import { MapState } from './map.js';
import { handleSubmit, saveCriteria } from './data.js';
import { addDropdown, initDropdowns, updateSubmitButtonState, showLoading, hideLoading, switchTab } from './ui.js';
// import { cluster } from '../node_modules/d3-hierarchy/dist/d3-hierarchy.js';

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
    document.getElementById('mapTabButton').addEventListener('click', () => switchTab('map'));
    document.getElementById('vennTabButton').addEventListener('click', () => switchTab('venn'));
    mapState.initMap();
});
// // Need a separate event listener to allow CSS render to finish
// window.addEventListener("load", () => {
//   createVennDiagram(data);
// });
