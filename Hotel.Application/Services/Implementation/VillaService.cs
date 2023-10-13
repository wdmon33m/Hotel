using Hotel.Application.Common.Dto;
using Hotel.Application.Common.Interfaces;
using Hotel.Application.Services.Interface;
using Hotel.Application.Utility;
using Hotel.Domain.Entities;

namespace Hotel.Application.Services.Implementation
{
    public class VillaService : IVillaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ResponseDto _response;

        public VillaService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _response = new();
        }

        public ResponseDto CreateVilla(Villa entity, string webRootPath)
        {
            try
            {
                if (entity.Image is not null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(entity.Image.FileName);
                    string imagePath = Path.Combine(webRootPath, @"images\VillaImages");

                    using var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create);
                    entity.Image.CopyTo(fileStream);

                    entity.ImageUrl = @"\images\VillaImages\" + fileName;
                }

                _unitOfWork.Villa.Add(entity);
                _unitOfWork.Save();

                return _response;
            }
            catch (Exception ex)
            {
                return _response.Exception(ex.Message);
            }
        }

        public ResponseDto DeleteVilla(int id, string webRootPath)
        {
            try
            {
                Villa? obj = GetVilla(id);

                if (!string.IsNullOrEmpty(obj.ImageUrl) && !obj.ImageUrl.Equals(@"https://placehold.co/600x400"))
                {
                    var oldImagePath = Path.Combine(webRootPath, obj.ImageUrl.TrimStart('\\'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                _unitOfWork.Villa.Remove(obj);
                _unitOfWork.Save();
                return _response;
            }
            catch (Exception ex)
            {
                return _response.Exception(ex.Message);
            }
        }

        public IEnumerable<Villa> GetAllVillas()
        {
            return _unitOfWork.Villa.GetAll();
        }

        public Villa GetVilla(int id)
        {
            return _unitOfWork.Villa.Get(e => e.Id == id);
        }

        public bool IsVillaAvailble(int villaId, DateOnly checkInDate, int nights)
        {
            try
            {
                var villaNumberList = _unitOfWork.VillaNumber.GetAll().ToList();
                var bookedVillas = _unitOfWork.Booking.GetAll(u => u.Status == SD.Status_Approved ||
                                    u.Status == SD.Status_CheckIn).ToList();

                List<int> bookingInDate = new();
                int finalAvailableRoomForAllNights = int.MaxValue;
                var roomsInVilla = villaNumberList.Where(x => x.VillaId == villaId).Count();

                for (int i = 0; i < nights; i++)
                {
                    var villasBooked = bookedVillas.Where(u => u.CheckInDate <= checkInDate.AddDays(i)
                    && u.CheckOutDate > checkInDate.AddDays(i) && u.VillaId == villaId);

                    foreach (var booking in villasBooked)
                    {
                        if (!bookingInDate.Contains(booking.Id))
                        {
                            bookingInDate.Add(booking.Id);
                        }
                    }

                    var totalAvailableRooms = roomsInVilla - bookingInDate.Count;
                    if (totalAvailableRooms == 0)
                    {
                        return false;
                    }
                    else
                    {
                        if (finalAvailableRoomForAllNights > totalAvailableRooms)
                        {
                            return true;
                        }
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public ResponseDto UpdateVilla(Villa entity, string webRootPath)
        {
            try
            {
                if (entity.Image is not null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(entity.Image.FileName);
                    string imagePath = Path.Combine(webRootPath, @"images\VillaImages");

                    if (!string.IsNullOrEmpty(entity.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(webRootPath, entity.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create);
                    entity.Image.CopyTo(fileStream);

                    entity.ImageUrl = @"\images\VillaImages\" + fileName;
                }
                _unitOfWork.Villa.Update(entity);
                _unitOfWork.Save();

                return _response;
            }
            catch (Exception ex)
            {
                return _response.Exception(ex.Message);
            }
        }
    }
}
