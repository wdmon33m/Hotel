$(document).ready(function () {
    loadTotalRevenueRadialChart();
});

function loadTotalRevenueRadialChart() {
    $(".chart-spinner").show();

    $.ajax({
        url: "/Dashboard/GetTotalRevenueChartData",
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            document.querySelector("#spanTotalRevenueCount").innerHTML = data.totalCount;

            var selectionCurrentCount = document.createElement("span");
            if (data.hasRatioIncreased) {
                selectionCurrentCount.className = "text-success me-1";
                selectionCurrentCount.innerHTML = '<i class="bi bi-arrow-up-right-circle me-1"></i> <span>' + data.countInCurrentMonth + '</span>';
            }
            else {
                selectionCurrentCount.className = "text-danger me-1";
                selectionCurrentCount.innerHTML = '<i class="bi bi-arrow-down-right-circle me-1"></i> <span>' + data.countInCurrentMonth + '</span>';
            }

            document.querySelector("#sectionRevenueCount").append(selectionCurrentCount);
            document.querySelector("#sectionRevenueCount").append("since last month");

            loadRedialBarChart("totalRevenuesRadialChart", data);

            $(".chart-spinner").hide();
        }
    });
}
