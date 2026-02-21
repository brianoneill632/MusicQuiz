document.addEventListener("DOMContentLoaded", function () {
    // Get data from the model
    const weeklyLoginCounts = JSON.parse(document.getElementById("weeklyLoginCounts").value);

    // Function to get the start date of the week
    function getWeekStartDate(weeksAgo) {
        const now = new Date();
        const dayOfWeek = now.getDay(); // 0 (Sunday) to 6 (Saturday)
        const diff = now.getDate() - dayOfWeek + (dayOfWeek === 0 ? -6 : 1) - (weeksAgo * 7); // Adjust when day is Sunday
        const weekStartDate = new Date(now.setDate(diff));
        return weekStartDate.toISOString().split('T')[0]; // Format as YYYY-MM-DD
    }

    // Calculate the start dates for the weeks
    const thisWeekStartDate = getWeekStartDate(0);
    const lastWeekStartDate = getWeekStartDate(1);
    const twoWeeksAgoStartDate = getWeekStartDate(2);
    const threeWeeksAgoStartDate = getWeekStartDate(3);

    // Chart data
    const data = {
        labels: [
            `This Week\n(${thisWeekStartDate})`,
            `Last Week\n(${lastWeekStartDate})`,
            `2 Weeks Ago\n(${twoWeeksAgoStartDate})`,
            `3 Weeks Ago\n(${threeWeeksAgoStartDate})`,
            "Older"
        ],
        datasets: [
            {
                label: "Logins Per Week",
                data: weeklyLoginCounts,
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
    const ctx = document.getElementById("loginsChart").getContext("2d");
    new Chart(ctx, {
        type: "bar",
        data: data,
        options: options
    });
});
