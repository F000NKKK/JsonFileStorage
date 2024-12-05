namespace TestSoft.FileStorageWebAPI.Contracts
{
    /// <summary>
    /// Represents a single patch operation for modifying a JSON object.
    /// </summary>
    public class JsonPatchOperationDto
    {
        /// <summary>
        /// Gets or sets the operation type: add, replace, or remove.
        /// </summary>
        public required string Op { get; set; }

        /// <summary>
        /// Gets or sets the path to the element in the JSON object.
        /// Example: "/property/nestedProperty".
        /// </summary>
        public required string Path { get; set; }

        /// <summary>
        /// Gets or sets the value that will be added or replaced in the JSON object (used for add/replace operations).
        /// </summary>
        public object? Value { get; set; }
    }
}
