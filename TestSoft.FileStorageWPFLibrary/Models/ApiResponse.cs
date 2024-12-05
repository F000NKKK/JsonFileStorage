namespace TestSoft.FileStorageWPFLibrary.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? ErrorMessage { get; set; }
        public int? ErrorCode { get; set; }

        public ApiResponse(T data)
        {
            Success = true;
            Data = data;
        }

        public ApiResponse(string errorMessage, int errorCode)
        {
            Success = false;
            ErrorMessage = errorMessage;
            ErrorCode = errorCode;
        }
    }

}
