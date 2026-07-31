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
        // Extract values handling both uppercase (from SQL) and lowercase keys
        const title = (movie.title || movie.Title || "").toLowerCase();
        const studio = (movie.studio || movie.Studio || "").toLowerCase();
        const release = (movie.release || movie.Release || "").toString();
        const location = (movie.location || movie.Location || "").toLowerCase();

        const matchesLocation = (selectedLocation === "all" || location === selectedLocation.toLowerCase());
        const matchesSearch = title.includes(searchTerm) || 
                              studio.includes(searchTerm) ||
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

    // --- NEW BIT HERE ---
    filtered.forEach(movie => {
        const title = movie.title || movie.Title || '';
        const release = movie.release || movie.Release || '';
        const location = movie.location || movie.Location || '';
        const studio = movie.studio || movie.Studio || '';
        const animated = movie.animated || movie.Animated || '';

        const row = document.createElement("tr");
        row.innerHTML = `
            <td class="col-title">${title}</td>
            <td>${release}</td>
            <td><span class="vhs-badge">${location}</span></td>
            <td>${studio}</td>
            <td><span class="anim-tag">${animated}</span></td>
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