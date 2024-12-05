namespace TestSoft.FileStorageWebAPI.Contracts
{
    /// <summary>
    /// Represents a request containing a list of patch operations to apply to a JSON object.
    /// </summary>
    public class JsonPatchRequestDto
    {
        /// <summary>
        /// Gets or sets the list of patch operations to apply to the JSON object.
        /// </summary>
        public List<JsonPatchOperationDto> Operations { get; set; } = new();
    }
}
