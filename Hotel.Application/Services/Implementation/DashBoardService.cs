using Hotel.Application.Common.Dto;
using Hotel.Application.Common.Interfaces;
using Hotel.Application.Services.Interface;
using Hotel.Application.Utility;

namespace Hotel.Application.Services.Implementation
{
    public class DashBoardService : IDashBoardService
    {
        private readonly IUnitOfWork _unitOfWork;
        static int previousMounth = DateTime.Now.Month == 1 ? 12 : DateTime.Now.Month - 1;
        private DateOnly previousMounthStartDate = new(DateTime.Now.Year, previousMounth, 1);
        private DateOnly currentMounthStartDate = new(DateTime.Now.Year, DateTime.Now.Month, 1);
        public DashBoardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<RadialBarChartDto> GetTotalBookingRadialChartData()
        {
            var totalBookings = _unitOfWork.Booking.GetAll(b => b.Status == SD.Status_Completed ||
            b.Status == SD.Status_CheckIn || b.Status == SD.Status_Approved);

            var countByCurrentMonth = totalBookings.Count(u => u.BookingDate >= currentMounthStartDate &&
            u.BookingDate <= DateTime.Now.ToDateOnly());

            var countByPreviousMonth = totalBookings.Count(u => u.BookingDate >= previousMounthStartDate &&
            u.BookingDate <= currentMounthStartDate);

            return SD.GetRadialCartDataModel(totalBookings.Count(), countByCurrentMonth, countByPreviousMonth);
        }

        public async Task<RadialBarChartDto> GetRegisteredUserChartData()
        {
            var totalUsers = _unitOfWork.User.GetAll();

            var countByCurrentMonth = totalUsers.Count(u => u.CreatedAt >= currentMounthStartDate &&
            u.CreatedAt <= DateTime.Now.ToDateOnly());

            var countByPreviousMonth = totalUsers.Count(u => u.CreatedAt >= previousMounthStartDate &&
            u.CreatedAt <= currentMounthStartDate);

            return SD.GetRadialCartDataModel(totalUsers.Count(), countByCurrentMonth, countByPreviousMonth);
        }

        public async Task<RadialBarChartDto> GetTotalRevenueChartData()
        {
            var totalBookings = _unitOfWork.Booking.GetAll(b => b.Status == SD.Status_Completed ||
            b.Status == SD.Status_CheckIn || b.Status == SD.Status_Approved);

            var totalRevenue = totalBookings.Sum(u => u.TotalCost);

            var countByCurrentMonth = totalBookings.Where(u => u.BookingDate >= currentMounthStartDate &&
            u.BookingDate <= DateTime.Now.ToDateOnly()).Sum(u => u.TotalCost);

            var countByPreviousMonth = totalBookings.Where(u => u.BookingDate >= previousMounthStartDate &&
            u.BookingDate <= currentMounthStartDate).Sum(u => u.TotalCost);

            return SD.GetRadialCartDataModel(totalRevenue, countByCurrentMonth, countByPreviousMonth);
        }
        
        public async Task<PieChartDto> GetBookingPieChartData()
        {
            var lastMonth = DateTime.Now.AddDays(-30).ToDateOnly();

            var totalBookings = _unitOfWork.Booking.GetAll(b => b.BookingDate >= lastMonth &&
                                b.Status == SD.Status_Completed || b.Status == SD.Status_CheckIn || b.Status == SD.Status_Approved);

            var customerWithOneBooking = totalBookings.GroupBy(b => b.UserId).Where(x => x.Count() == 1).Select(k => k.Key).ToList();

            int bookingsByNewCustomer = customerWithOneBooking.Count();
            int bookingByReturningCustomer = totalBookings.Count() - bookingsByNewCustomer;

            PieChartDto pieChartVM = new()
            {
                Labels = new string[] { "New Customer Bookings", "Returning Customer Bookings" },
                Series = new decimal[] { bookingsByNewCustomer, bookingByReturningCustomer }
            };

            return pieChartVM;
        }

        public async Task<LineChartDto> GetMemberAndBookingLineChartData()
        {
            var lastMonth = DateTime.Now.AddDays(-30).ToDateOnly();
            var bookingData = _unitOfWork.Booking.GetAll(b => b.BookingDate >= lastMonth)
                              .GroupBy(b => b.BookingDate)
                              .Select(u => new
                              {
                                  Date = u.Key,
                                  NewBookingCount = u.Count()
                              });

            var customerData = _unitOfWork.User.GetAll(b => b.CreatedAt >= lastMonth)
                              .GroupBy(b => b.CreatedAt)
                              .Select(u => new
                              {
                                  Date = u.Key,
                                  NewCustomerCount = u.Count()
                              });

            var leftJoin = bookingData.GroupJoin(customerData, booking => booking.Date, customer => customer.Date,
                            (booking, customer) => new
                            {
                                booking.Date,
                                booking.NewBookingCount,
                                NewCustomerCount = customer.Select(x => x.NewCustomerCount).FirstOrDefault()
                            });

            var rightJoin = customerData.GroupJoin(bookingData, customer => customer.Date, booking => booking.Date,
                            (customer, booking) => new
                            {
                                customer.Date,
                                NewBookingCount = booking.Select(x => x.NewBookingCount).FirstOrDefault(),
                                customer.NewCustomerCount
                            });

            var mergedData = leftJoin.Union(rightJoin).OrderBy(x => x.Date).ToList();

            var newBookingData = mergedData.Select(x => x.NewBookingCount).ToArray();
            var newCustomerData = mergedData.Select(x => x.NewCustomerCount).ToArray();
            var categories = mergedData.Select(x => x.Date.ToString()).ToArray();

            List<ChartData> chartData = new()
            {
                new ChartData
                {
                   Name = "New Bookings",
                   Data = newBookingData
                },
                new ChartData
                {
                   Name = "New Costomers",
                   Data = newCustomerData
                }
            };

            LineChartDto lineChartVM = new()
            {
                Categories = categories,
                Series = chartData
            };

            return lineChartVM;
        }
    }
}
