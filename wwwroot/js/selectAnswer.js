function selectOption(optionId, className) {
    // Remove 'selected' class from all options with the specified className
    document.querySelectorAll(`.${className}`).forEach(div => {
        div.classList.remove('selected');
    });

    // Add 'selected' class to the clicked option
    document.getElementById(optionId).classList.add('selected');

    // Set the value of the hidden input field
    if (document.getElementById('selectedOption')) {
        document.getElementById('selectedOption').value = optionId;
    }

    // Also check the corresponding radio button
    const radioInput = document.querySelector(`#${optionId} input[type="radio"]`);
    if (radioInput) {
        radioInput.checked = true;
    }
}

document.addEventListener('DOMContentLoaded', function () {
    // Handle clicks on all quiz options (for topic selection, difficulty selection, and quiz answers)
    document.querySelectorAll('.quiz-option, .topic-card, .difficulty-card').forEach(function (element) {
        if (!element.classList.contains('disabled')) {
            element.addEventListener('click', function () {
                const className = this.classList.contains('quiz-option') ? 'quiz-option' :
                    (this.classList.contains('topic-card') ? 'topic-card' : 'difficulty-card');

                // Remove 'selected' class from all elements with the same class
                document.querySelectorAll(`.${className}`).forEach(function (el) {
                    el.classList.remove('selected');
                });

                // Add 'selected' class to the clicked element
                this.classList.add('selected');

                // If there's a radio button inside, check it
                const radio = this.querySelector('input[type="radio"]');
                if (radio) {
                    radio.checked = true;
                }

                // If there's a hidden input with id 'selectedOption', update its value with the element's id
                const hiddenInput = document.getElementById('selectedOption');
                if (hiddenInput) {
                    hiddenInput.value = this.id;
                }

                // For topic cards and difficulty cards, also handle specific hidden inputs
                if (this.classList.contains('topic-card')) {
                    const selectedTopicInput = document.getElementById('selectedTopic');
                    if (selectedTopicInput) {
                        selectedTopicInput.value = this.id;
                    }
                } else if (this.classList.contains('difficulty-card')) {
                    const selectedDifficultyInput = document.getElementById('selectedDifficulty');
                    if (selectedDifficultyInput) {
                        selectedDifficultyInput.value = this.id;
                        // Enable submit button on difficulty pages
                        const submitButton = document.getElementById('submitButton');
                        if (submitButton) {
                            submitButton.disabled = false;
                        }
                    }
                }
            });
        }
    });

    // Ensure existing selections are visually indicated on page load
    const preSelectedOptions = document.querySelectorAll('input[type="radio"]:checked');
    preSelectedOptions.forEach(function (radio) {
        const parentElement = radio.closest('.quiz-option, .topic-card, .difficulty-card');
        if (parentElement) {
            parentElement.classList.add('selected');
        }
    });
});
