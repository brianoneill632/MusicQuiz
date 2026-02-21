async function uploadFile() {
    const fileInput = document.getElementById('fileInput');
    const formData = new FormData();
    formData.append('file', fileInput.files[0]);

    const response = await fetch('/Admin/UploadMusicFile', {
        method: 'POST',
        body: formData
    });

    if (response.ok) {
        alert('File uploaded successfully');
        loadMusicFiles();
    } else {
        alert('Failed to upload file');
    }
}

async function loadMusicFiles() {
    const response = await fetch('/Admin/GetMusicFiles');
    const musicFiles = await response.json();

    const dropdown = document.getElementById('musicFilesDropdown');
    dropdown.innerHTML = '<option value="">Select a music file</option>';
    musicFiles.forEach(file => {
        const option = document.createElement('option');
        option.value = file;
        option.textContent = file;
        dropdown.appendChild(option);
    });
}

async function deleteFile() {
    const dropdown = document.getElementById('musicFilesDropdown');
    const selectedFile = dropdown.value;

    if (!selectedFile) {
        alert('Please select a file to delete');
        return;
    }

    const confirmation = confirm('This will delete the music file along with all corresponding questions. Do you want to proceed?');
    if (!confirmation) {
        return;
    }

    const response = await fetch(`/Admin/DeleteMusicFile?fileName=${encodeURIComponent(selectedFile)}`, {
        method: 'DELETE'
    });

    if (response.ok) {
        alert('File and corresponding questions deleted successfully');
        loadMusicFiles();
    } else {
        alert('Failed to delete file and corresponding questions');
    }
}

document.addEventListener('DOMContentLoaded', loadMusicFiles);


