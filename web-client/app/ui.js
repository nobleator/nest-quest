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
    const vennTab = document.getElementById('vennTabButton');
    const mapContainer = document.getElementById('mapContainer');
    const vennContainer = document.getElementById('vennContainer');
    if (tab === 'map') {
        vennContainer.classList.remove('active');
        mapContainer.classList.add('active');
        mapTab.classList.add('active');
        vennTab.classList.remove('active');
    } else if (tab === 'venn') {
        mapContainer.classList.remove('active');
        vennContainer.classList.add('active');
        vennTab.classList.add('active');
        mapTab.classList.remove('active');
        createVennDiagram(data);
    }
}

/*
Venn diagram logic
1) Start with a dataset that provides N nodes with set membership defined
2) Using https://www.benfrederickson.com/venn-diagrams-with-d3.js/ and the associated library (or a fork like https://github.com/upsetjs/venn.js), define Venn diagram circles for each set, where the proportional sizes correspond to the number of nodes within that set. May need to offset by a fixed amount for non-intersectional sets to force everything to render properly.
3) Use the d3.packSiblings function (https://using-d3js.com/06_05_packs.html#h_BYU7pg5Wrk) for each set, passing the member nodes with all the same radii of 1.
4) Find the center of each set from 2), and add the packed nodes from 3) centered around the center of the set.
5) Place the final resulting diagram within the <div> with ID "venn".

*/

const data = [
    { id: 1, label: '1', sets: ['A'] },
    { id: 2, label: '2', sets: ['A', 'B', 'C'] },
    { id: 3, label: '3', sets: ['A', 'B'] },
    { id: 4, label: '4', sets: ['B'] },
    { id: 5, label: '5', sets: ['B', 'C'] },
    { id: 6, label: '6', sets: ['A', 'C'] },
    { id: 7, label: '7', sets: ['A'] },
    { id: 8, label: '8', sets: ['C'] },
    { id: 9, label: '9', sets: ['B'] },
    { id: 10, label: '10', sets: ['C'] },
];

function countSets(data) {
    const setCounts = {};
    data.forEach(item => {
        const sortedSets = item.sets.slice().sort();
        const key = JSON.stringify(sortedSets);
        if (setCounts[key]) {
            setCounts[key] += 1;
        } else {
            setCounts[key] = 1;
        }
    });
    return Object.keys(setCounts).map(key => {
        return { sets: JSON.parse(key), size: setCounts[key] };
    });
}

function filterNodesBySet(data, targetSet) {
    const sortedTargetSet = targetSet.slice().sort();
    return data.filter(node => {
        const sortedNodeSets = node.sets.slice().sort();
        return JSON.stringify(sortedTargetSet) === JSON.stringify(sortedNodeSets);
    });
}

function addNodeSets(sets) {
    console.log('### applying node sets to venn...');
    sets.map(s => {
        const setId = s.sets.join('_');
        const vennElement = document.querySelector(`#vennDiv > svg > g[data-venn-sets="${setId}"]`);
        const path = document.querySelector(`#vennDiv > svg > g[data-venn-sets="${setId}"] > path`);
        console.log(`element found for set ID ${setId}:`);
        console.log(vennElement);
        console.log(path);
        const startPoint = path.getPointAtLength(0);
        console.log(`Start point: x=${startPoint.x}, y=${startPoint.y}`);
        const bbox = path.getBBox();
        const centerX = bbox.x + bbox.width / 2;
        const centerY = bbox.y + bbox.height / 2;
        console.log(`bbox center: x=${centerX}, y=${centerY}`);
        const nodes = filterNodesBySet(data, s.sets);
        console.log(`nodes:`);
        console.log(nodes);
        const circles = nodes.map(n => ({ id: n.id, r: 5 }));
        circles.sort((a,b) => b.r - a.r);
        console.log(circles);
        const svg = d3.select(vennElement);
            // .attr('transform', 'translate(250, 250)');
        const root = svg.append('g')
            .attr('transform', `translate(${centerX}, ${centerY})`);
        const drawCircle = (d, color) => {
            console.log('drawing node')
            console.log(d);
            root.append("circle")
                // .attr('transform', `translate(${startPoint.x}, ${startPoint.y})`)
                // .attr('transform', `translate(${centerX}, ${centerY})`)
                .attr("cx", d.x)
                .attr("cy", d.y)
                .attr("r", d.r)
                .attr("fill", color);
            root.append("text")
                .attr("x", d.x)
                .attr("y", d.y)
                .attr("dy", "0.35em")
                .attr("text-anchor", "middle")
                .attr("fill", "black")
                .style("font-size", "0.5em")
                .text(d.id);
        }
        console.log('packing circles...');
        const packCircles = d3.packSiblings(circles);
        console.log(packCircles);
        console.log('placing packed circles...');
        packCircles.map(d => drawCircle(d, '#f0f'));   
        console.log('done placing circles');
    });
    console.log('### nodes sets applied.');
}

function createVennDiagram(data) {
    console.log('START: source data:');
    console.log(data);
    const sets = countSets(data);
    console.log('sets constructed');
    console.log(sets);
    console.log('creating base layer venn...');
    const chart = venn.VennDiagram();
    d3.select('#vennDiv').datum(sets).call(chart);

    const regions = sets.map((region) => {
        const path = venn.intersectionAreaPath(region.sets.map((set) => sets.find(s => s.sets.includes(set))));
        return {
            sets: region.sets,
            path,
        };
    });
    
    addNodeSets(sets);

    // console.log('creating standalone packed circles...');
    // const circles = data2.map(item => ({ r: item.id * 10 }));
    // circles.sort((a,b) => b.r - a.r);
    // console.log(circles);
    // #venn is an <svg>, while vennElement is a <g> within the <svg> created in the #vennDiagram
    // const svg = d3.select("#venn");
    // const root = svg.append('g')
    //     .attr('transform', 'translate(250, 250)');
    // console.log('packing circles...');
    // const packCircles = d3.packSiblings(circles);
    // console.log(packCircles);
    // const center = d3.packEnclose(packCircles);
    // const drawCircle = (d, color) => {
    //     console.log(d)
    //     // root
    //     //     .append("circle")
    //     //     .attr("cx", d.x)
    //     //     .attr("cy", d.y)
    //     //     .attr("r", d.r)
    //     //     .attr("fill", color);
    // }
    // drawCircle(center, '#eee');
    // packCircles.map(d => drawCircle(d, '#f0f'));      
}