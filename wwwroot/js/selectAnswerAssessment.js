function selectOption(optionValue, className) {
    // Remove 'selected' class from all options
    document.querySelectorAll(`.${className}`).forEach(label => {
        label.classList.remove('selected');
    });

    // Find the label with the matching radio button and add 'selected' class
    document.querySelectorAll(`.${className}`).forEach(label => {
        const radio = label.querySelector('input[type="radio"]');
        if (radio && radio.value === optionValue) {
            label.classList.add('selected');
            radio.checked = true;
        }
    });

    // Set the value of the hidden input field
    const hiddenInput = document.getElementById('selectedOption');
    if (hiddenInput) {
        hiddenInput.value = optionValue;
    }

    // Enable the "Next" button
    const nextButton = document.getElementById('nextButton');
    if (nextButton) {
        nextButton.disabled = false;
    }
}

document.addEventListener('DOMContentLoaded', function () {
    // Add click event listeners to all quiz options
    document.querySelectorAll('.quiz-option').forEach(function (label) {
        label.addEventListener('click', function (e) {
            // If the user clicked directly on the label (not on the radio button)
            // we need to manually set the radio button
            const radio = label.querySelector('input[type="radio"]');
            if (radio && !radio.checked) {
                radio.checked = true;

                // Call selectOption with the radio value
                selectOption(radio.value, 'quiz-option');
            }
        });
    });

    // Also add direct listeners to radio buttons to handle their change events
    document.querySelectorAll('.quiz-radio').forEach(function (radio) {
        radio.addEventListener('change', function () {
            selectOption(this.value, 'quiz-option');
        });
    });

    // Check if there's already a selected answer (for when returning to a question)
    const selectedRadio = document.querySelector('input[type="radio"]:checked');
    if (selectedRadio) {
        // Enable the next button
        const nextButton = document.getElementById('nextButton');
        if (nextButton) {
            nextButton.disabled = false;
        }
    }
});
