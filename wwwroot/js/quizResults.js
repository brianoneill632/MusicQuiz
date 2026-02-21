document.addEventListener("DOMContentLoaded", function () {
    // Chart data
    const data = {
        labels: ["Right 1st Time", "Right 2nd Time", "Right 3rd Time", "Right 4th Time", "Incorrect"],
        datasets: [
            {
                label: "Quiz Results",
                data: [rightFirstTime, rightSecondTime, rightThirdTime, rightFourthTime, incorrectAnswers],
                backgroundColor: [
                    "#4CAF50", "#FFC107", "#FF9800", "#FF5722", "#F44336"
                ],
                borderRadius: 8,
                borderWidth: 1,
                hoverBackgroundColor: [
                    "#66BB6A", "#FFD54F", "#FFB74D", "#FF8A65", "#E57373"
                ],
                borderColor: "rgba(0,0,0,0.1)"
            }
        ]
    };

    // Chart options
    const options = {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
            legend: { display: false },
            tooltip: {
                backgroundColor: "rgba(0, 0, 0, 0.7)",
                titleColor: "#FFF",
                bodyColor: "#FFF",
                borderColor: "#CCC",
                borderWidth: 1,
                cornerRadius: 6
            }
        },
        layout: { padding: 0 },
        scales: {
            x: {
                beginAtZero: true,
                grid: { display: false },
                ticks: {
                    color: "#666",
                    font: { size: 14, weight: "bold" }
                }
            },
            y: {
                beginAtZero: true,
                max: totalQuestions,
                grid: {
                    color: "#E0E0E0",
                    borderDash: [5, 5]
                },
                ticks: {
                    stepSize: 1,
                    color: "#666",
                    font: { size: 14 }
                }
            }
        }
    };

    // Set chart size based on screen size
    const chartContainer = document.getElementById("chartContainer");

    function setChartSize() {
        if (window.innerWidth <= 768) {
            // For mobile screens
            chartContainer.style.width = "100%";
            chartContainer.style.height = "50vh";
        } else {
            // For desktop screens
            chartContainer.style.width = "80%";
            chartContainer.style.height = "400px";
        }
    }

    // Resizing screen event listener
    setChartSize();
    window.addEventListener("resize", setChartSize);

    // Create chart
    const ctx = document.getElementById("scoreChart").getContext("2d");
    new Chart(ctx, {
        type: "bar",
        data: data,
        options: options
    });
});
