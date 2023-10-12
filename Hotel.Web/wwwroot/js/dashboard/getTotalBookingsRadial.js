﻿$(document).ready(function () {
    loadTotalBookingRadialChart();
});

function loadTotalBookingRadialChart() {
    $(".chart-spinner").show();

    $.ajax({
        url: "/Dashboard/GetTotalBookingRadialChartData",
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            document.querySelector("#spanTotalBookingCount").innerHTML = data.totalCount;

            var selectionCurrentCount = document.createElement("span");
            if (data.hasRatioIncreased) {
                selectionCurrentCount.className = "text-success me-1";
                selectionCurrentCount.innerHTML = '<i class="bi bi-arrow-up-right-circle me-1"></i> <span>' + data.countInCurrentMonth + '</span>';
            }
            else {
                selectionCurrentCount.className = "text-danger me-1";
                selectionCurrentCount.innerHTML = '<i class="bi bi-arrow-down-right-circle me-1"></i> <span>' + data.countInCurrentMonth + '</span>';
            }

            document.querySelector("#sectionBookingCount").append(selectionCurrentCount);
            document.querySelector("#sectionBookingCount").append("since last month");

            loadRedialBarChart("totalBookingsRadialChart", data);

            $(".chart-spinner").hide();
        }
    });
}
