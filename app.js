let movies = [];
let selectedLocation = "all";

// DOM Elements
const searchInput = document.getElementById("searchInput");
const movieList = document.getElementById("movieList");
const locationBtns = document.querySelectorAll(".location-btn");
const locationHeader = document.getElementById("currentLocationHeader");
const noResults = document.getElementById("noResults");

// 1. Fetch movies from MS SQL via Python backend
async function fetchMovies() {
    try {
        const response = await fetch('/api/movies');
        if (!response.ok) throw new Error('Failed to fetch movies from database');
        
        movies = await response.json();
        renderMovies();
    } catch (error) {
        console.error('Error fetching movies:', error);
        noResults.textContent = "*** ERROR CONNECTING TO DATABASE ***";
        noResults.style.display = "block";
    }
}

// 2. Render and filter movies
function renderMovies() {
    const searchTerm = searchInput.value.toLowerCase().trim();

    const filtered = movies.filter(movie => {
        const title = (movie.title || movie.Title || "").toLowerCase().trim();
        const release = (movie.release || movie.Release || "").toString().trim();
        const location = (movie.location || movie.Location || "").toLowerCase().trim();
        const category = (movie.category || movie.Category || "").toLowerCase().trim();
        const studio = (movie.studio || movie.Studio || "").toLowerCase().trim();

        const matchesLocation = (selectedLocation === "all" || location === selectedLocation.toLowerCase());
        const matchesSearch = title.includes(searchTerm) || 
                              studio.includes(searchTerm) ||
                              category.includes(searchTerm) ||
                              release.includes(searchTerm);

        return matchesLocation && matchesSearch;
    });

    movieList.innerHTML = "";

    if (filtered.length === 0) {
        noResults.textContent = "*** NO VHS TAPES FOUND MATCHING QUERY ***";
        noResults.style.display = "block";
        return;
    }

    noResults.style.display = "none";
    filtered.forEach(movie => {
        const title = (movie.Title || '').toString().trim();
        const release = (movie.Release || '').toString().trim();
        const location = (movie.Location || '').toString().trim();
        const category = (movie.Category || '').toString().trim();
        const studio = (movie.Studio || '').toString().trim();

        const row = document.createElement("tr");
        // Position 4 = studio ("DISNEY") -> Matches CATEGORY header column index if swapped, or alignment
        // Position 5 = category ("ANIMATION") 
        row.innerHTML = `
            <td class="col-title">${title}</td>
            <td>${release}</td>
            <td><span class="vhs-badge">${location}</span></td>
            <td>${studio}</td>
            <td><span class="category-tag">${category}</span></td>
        `;
        movieList.appendChild(row);
    });
}

// Event Listeners
searchInput.addEventListener("input", renderMovies);

locationBtns.forEach(btn => {
    btn.addEventListener("click", () => {
        locationBtns.forEach(b => b.classList.remove("active"));
        btn.classList.add("active");

        selectedLocation = btn.dataset.location;
        locationHeader.textContent = selectedLocation === "all" 
            ? "CATALOG: ALL LOCATIONS" 
            : `CATALOG: ${selectedLocation.toUpperCase()}`;

        renderMovies();
    });
});

// Load movies when the page loads
fetchMovies();
