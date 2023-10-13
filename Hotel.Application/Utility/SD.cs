using Hotel.Application.Common.Dto;
using Hotel.Domain.Entities;

namespace Hotel.Application.Utility
{
    public static class SD
    {
        public const string Role_Admin = "Admin";
        public const string Role_Customer = "Customer";

        public const string AccessDeniedPath = "/Account/AccessDenied";
        public const string LoginPath = "/Account/Login";

        public const string Status_Pending = "Pending";
        public const string Status_Approved = "Approved";
        public const string Status_CheckIn = "CheckIn";
        public const string Status_Completed = "Completed";
        public const string Status_Cancelled = "Cancelled";
        public const string Status_Refunded = "Refunded";

        public static DateOnly ToDateOnly(this DateTime dateTime)
        {
            return DateOnly.FromDateTime(dateTime);
        }
        public static DateTime ToDateTimeOnly(this DateOnly date)
        {
            TimeSpan timeNow = DateTime.Now.TimeOfDay;
            TimeOnly timeOnly = new TimeOnly(timeNow.Hours,timeNow.Minutes,timeNow.Seconds);

            return date.ToDateTime(timeOnly);
        }

        public static RadialBarChartDto GetRadialCartDataModel(decimal totalCount, decimal currentMonthCount, decimal previousMonthCount)
        {
            RadialBarChartDto radialBarChartVM = new();

            int IncreaseDecreaseRatio = 100;

            if (previousMonthCount != 0)
            {
                IncreaseDecreaseRatio = Convert.ToInt32((currentMonthCount - previousMonthCount) / previousMonthCount * 100);
            }

            radialBarChartVM.TotalCount = totalCount;
            radialBarChartVM.CountInCurrentMonth = currentMonthCount;
            radialBarChartVM.HasRatioIncreased = currentMonthCount > previousMonthCount;
            radialBarChartVM.Series = new int[] { IncreaseDecreaseRatio };

            return radialBarChartVM;
        }
    }
}
