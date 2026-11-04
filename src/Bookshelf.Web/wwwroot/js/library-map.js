const map = L.map('libraryMap').setView([50.45, 30.52], 11);

L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
    maxZoom: 19,
    attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
}).addTo(map);

const markers = L.layerGroup().addTo(map);
const bookSelect = document.getElementById('filterBook');
const districtSelect = document.getElementById('filterDistrict');
const counter = document.getElementById('libraryCount');

function escapeHtml(text) {
    return text
        .replaceAll('&', '&amp;')
        .replaceAll('<', '&lt;')
        .replaceAll('>', '&gt;')
        .replaceAll('"', '&quot;')
        .replaceAll("'", '&#39;');
}

async function loadLibraries() {
    const params = new URLSearchParams();
    if (bookSelect.value) params.append('bookId', bookSelect.value);
    if (districtSelect.value) params.append('district', districtSelect.value);

    const response = await fetch('/api/libraries/map?' + params);
    const libraries = await response.json();

    markers.clearLayers();
    counter.textContent = libraries.length;

    for (const library of libraries) {
        let html = '<strong>' + escapeHtml(library.name) + '</strong><br>'
            + escapeHtml(library.address) + '<br>'
            + 'Книг: ' + library.bookCount
            + '<ul class="mb-0 ps-3">';
        for (const book of library.books) {
            html += '<li><a href="/Books/Details/' + book.id + '">' + escapeHtml(book.title) + '</a></li>';
        }
        html += '</ul>';

        L.marker([library.latitude, library.longitude]).bindPopup(html).addTo(markers);
    }
}

bookSelect.addEventListener('change', loadLibraries);
districtSelect.addEventListener('change', loadLibraries);
loadLibraries();
