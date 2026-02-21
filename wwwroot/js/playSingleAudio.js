// audioPlayerControl.js
document.addEventListener('DOMContentLoaded', function () {
    // Get all audio elements on the page
    const audioElements = document.querySelectorAll('audio');

    // Add event listeners to each audio element
    audioElements.forEach(audio => {
        // When one audio starts playing, pause all others
        audio.addEventListener('play', function () {
            audioElements.forEach(otherAudio => {
                if (otherAudio !== audio && !otherAudio.paused) {
                    otherAudio.pause();
                }
            });
        });
    });
});