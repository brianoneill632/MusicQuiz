function submitAnswer() {
    var selectedOption = document.querySelector('input[name="selectedOption"]:checked');
    if (!selectedOption) {
        alert("Please select an option.");
        return;
    }

    var correctAnswer = document.querySelector('input[name="correctAnswer"]').value;

    // Remove previous result classes from all options
    var options = document.querySelectorAll('.quiz-option');
    options.forEach(function (option) {
        // Remove result classes but keep selection class if applicable
        option.classList.remove('correct-answer', 'incorrect-answer');
    });

    // Get feedback container
    var feedbackSection = document.getElementById('feedbackSection');

    // Clear any existing feedback
    feedbackSection.innerHTML = '';

    // Apply appropriate class to the selected option
    var parentOption = selectedOption.closest('.quiz-option');

    // Create feedback message in the dedicated feedback section
    var feedbackMessage = document.createElement('div');
    feedbackMessage.classList.add('feedback-message');

    if (selectedOption.value === correctAnswer) {
        // Keep the selected class for neutral highlighting and add result class
        parentOption.classList.add('correct-answer');
        feedbackMessage.classList.add('feedback-correct');
        feedbackMessage.innerHTML = '<i class="fas fa-check-circle me-2"></i> Correct! That\'s the right answer.';
        // Add green styling to feedback
        feedbackSection.style.backgroundColor = '#d4edda';
        feedbackSection.style.color = '#155724';
        feedbackSection.style.border = '1px solid #c3e6cb';
    } else {
        // Keep the selected class for neutral highlighting and add result class
        parentOption.classList.add('incorrect-answer');
        feedbackMessage.classList.add('feedback-incorrect');
        feedbackMessage.innerHTML = '<i class="fas fa-times-circle me-2"></i> That\'s not correct. Try again.';
        // Add red styling to feedback
        feedbackSection.style.backgroundColor = '#f8d7da';
        feedbackSection.style.color = '#721c24';
        feedbackSection.style.border = '1px solid #f5c6cb';
    }

    // Add the feedback to the dedicated feedback section
    feedbackSection.appendChild(feedbackMessage);

    // Update attempt counter
    var attemptNumberField = document.getElementById('attemptNumber');
    var attemptNumber = parseInt(attemptNumberField.value) || 0;
    attemptNumber++;
    attemptNumberField.value = attemptNumber;

    // Store first answer for scoring purposes
    var firstUserAnswerField = document.getElementById('firstUserAnswer');
    if (!firstUserAnswerField.value) {
        firstUserAnswerField.value = selectedOption.value;
    }

    // Enable the next button
    document.getElementById('nextButton').disabled = false;
}

// Update the DOMContentLoaded event handler to use the feedback section
document.addEventListener("DOMContentLoaded", function () {
    var feedbackValue = document.getElementById("feedback").value;
    if (feedbackValue) {
        var selectedOption = document.querySelector('input[name="selectedOption"]:checked');
        if (selectedOption) {
            // Mark selected option with appropriate class
            var parentOption = selectedOption.closest('.quiz-option');

            // Use result-specific classes instead of correct/incorrect
            if (feedbackValue.includes('Correct')) {
                parentOption.classList.add('correct-answer');
            } else {
                parentOption.classList.add('incorrect-answer');
            }

            // Display feedback in the dedicated section
            var feedbackSection = document.getElementById('feedbackSection');
            var feedbackMessage = document.createElement('div');
            feedbackMessage.classList.add('feedback-message');

            if (feedbackValue.includes('Correct')) {
                feedbackMessage.classList.add('feedback-correct');
                feedbackMessage.innerHTML = '<i class="fas fa-check-circle me-2"></i> ' + feedbackValue;
                // Add green styling to feedback
                feedbackSection.style.backgroundColor = '#d4edda';
                feedbackSection.style.color = '#155724';
                feedbackSection.style.border = '1px solid #c3e6cb';
            } else {
                feedbackMessage.classList.add('feedback-incorrect');
                feedbackMessage.innerHTML = '<i class="fas fa-times-circle me-2"></i> ' + feedbackValue;
                // Add red styling to feedback
                feedbackSection.style.backgroundColor = '#f8d7da';
                feedbackSection.style.color = '#721c24';
                feedbackSection.style.border = '1px solid #f5c6cb';
            }

            feedbackSection.appendChild(feedbackMessage);

            // Enable the next button
            document.getElementById('nextButton').disabled = false;
        }
    }
});
