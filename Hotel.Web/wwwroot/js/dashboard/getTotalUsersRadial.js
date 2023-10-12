$(document).ready(function () {
    loadTotalUseRadialChart();
});

function loadTotalUseRadialChart() {
    $(".chart-spinner").show();

    $.ajax({
        url: "/Dashboard/GetRegisteredUserChartData",
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            document.querySelector("#spanTotalUserCount").innerHTML = data.totalCount;

            var selectionCurrentCount = document.createElement("span");
            if (data.hasRatioIncreased) {
                selectionCurrentCount.className = "text-success me-1";
                selectionCurrentCount.innerHTML = '<i class="bi bi-arrow-up-right-circle me-1"></i> <span>' + data.countInCurrentMonth + '</span>';
            }
            else {
                selectionCurrentCount.className = "text-danger me-1";
                selectionCurrentCount.innerHTML = '<i class="bi bi-arrow-down-right-circle me-1"></i> <span>' + data.countInCurrentMonth + '</span>';
            }

            document.querySelector("#sectionUserCount").append(selectionCurrentCount);
            document.querySelector("#sectionUserCount").append("since last month");

            loadRedialBarChart("totalUsersRadialChart", data);

            $(".chart-spinner").hide();
        }
    });
}
