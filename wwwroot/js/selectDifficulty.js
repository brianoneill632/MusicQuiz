function selectOption(optionId, className) {
    // Remove 'selected' class from all options
    document.querySelectorAll(`.${className}`).forEach(div => {
        div.classList.remove('selected');
    });

    // Add 'selected' class to the clicked option
    document.getElementById(optionId).classList.add('selected');

    // Set the value of the hidden input field
    document.getElementById('selectedDifficulty').value = optionId;
}

