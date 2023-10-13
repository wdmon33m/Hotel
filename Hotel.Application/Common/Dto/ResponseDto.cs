using System.Net;

namespace Hotel.Application.Common.Dto
{
    public class ResponseDto
    {
        public bool IsSuccess { get; set; } = true;
        public string ErrorMessage { get; set; }
        public object Result { get; set; }

        public ResponseDto Exception(string? errorMessage = null)
        {
            IsSuccess = false;
            ErrorMessage = errorMessage;
            return this;
        }
    }
}
