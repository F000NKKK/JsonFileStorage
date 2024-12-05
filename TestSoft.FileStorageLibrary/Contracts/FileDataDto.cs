namespace TestSoft.FileStorageLibrary.Contracts
{
    /// <summary>
    /// Represents the data for a file stored in the file storage system.
    /// </summary>
    public class FileDataDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the file.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the data associated with the file.
        /// The data is stored as key-value pairs in a dictionary, where the key is a string
        /// and the value is an object that can represent various data types.
        /// </summary>
        public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();
    }
}
