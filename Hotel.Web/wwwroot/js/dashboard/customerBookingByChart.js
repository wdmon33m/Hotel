$(document).ready(function () {
    customerBookingByChart();
});

function customerBookingByChart() {
    $(".chart-spinner").show();

    $.ajax({
        url: "/Dashboard/GetBookingPieChartData",
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            loadPieChart("customerBookingsPieChart", data);

            $(".chart-spinner").hide();
        }
    });
}

function loadPieChart(id, data) {
    var chartColors = getChartColorsArray(id);

    var options = {
        colors: chartColors,
        series: data.series,
        labels: data.labels,
        chart: {
            type: 'pie',
            width: 380
        },
        stroke: {
            show: false
        },
        legend: {
            position: 'bottom',
            horizontalAlign: 'center',
            labels: {
                colors: "#fff",
                useSeriesColors: true
            }
        }
    };

    var chart = new ApexCharts(document.querySelector("#" + id), options);
    chart.render();
}
