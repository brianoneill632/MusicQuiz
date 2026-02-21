async function loadMusicFiles() {
    console.log('Loading music files...');
    try {
        const response = await fetch('/Admin/GetMusicFiles');
        if (!response.ok) {
            throw new Error(`Failed to fetch: ${response.statusText}`);
        }

        const musicFiles = await response.json();
        console.log('Music files:', musicFiles);

        const musicQuestionDropdown = document.getElementById('musicQuestionFilePathDropdown');
        const musicReferenceDropdown = document.getElementById('musicReferenceFilePathDropdown');

        if (!musicQuestionDropdown || !musicReferenceDropdown) {
            console.error('Dropdown elements not found.');
            return;
        }

        musicFiles.forEach(file => {
            const option = document.createElement('option');
            option.value = file;
            option.textContent = file;
            musicQuestionDropdown.appendChild(option);
            musicReferenceDropdown.appendChild(option.cloneNode(true));
        });

    } catch (error) {
        console.error('Error loading music files:', error);
    }
}

// Call the function when the DOM is fully loaded
document.addEventListener('DOMContentLoaded', loadMusicFiles);
