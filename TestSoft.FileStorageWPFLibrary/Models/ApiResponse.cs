namespace TestSoft.FileStorageWPFLibrary.Models
{
    /// <summary>
    /// Represents the response from an API call, including the result data or error details.
    /// </summary>
    /// <typeparam name="T">The type of the data returned in the response.</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Gets or sets a value indicating whether the operation was successful.
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Gets or sets the data returned by the API if the operation was successful.
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Gets or sets the error message if the operation was not successful.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the error code if the operation failed.
        /// </summary>
        public int? ErrorCode { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResponse{T}"/> class with success status and data.
        /// </summary>
        /// <param name="data">The data to return in the response.</param>
        public ApiResponse(T data)
        {
            IsSuccess = true;
            Data = data;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResponse{T}"/> class with failure status and error details.
        /// </summary>
        /// <param name="errorMessage">The error message to return in the response.</param>
        /// <param name="errorCode">The error code associated with the failure.</param>
        public ApiResponse(string errorMessage, int errorCode)
        {
            IsSuccess = false;
            ErrorMessage = errorMessage;
            ErrorCode = errorCode;
        }
    }
}
