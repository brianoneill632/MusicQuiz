document.addEventListener("DOMContentLoaded", function () {
    const successMessage = document.getElementById("successMessage");
    if (successMessage) {
        alert(successMessage.value);
    }
});

function handleFormSubmit(event) {
    event.preventDefault();
    alert('Question added successfully');
    document.getElementById('addQuestionForm').submit();
}
