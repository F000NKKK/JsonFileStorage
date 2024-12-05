namespace TestSoft.FileStorageWPFLibrary.Contracts
{
    /// <summary>
    /// Represents a JSON object with key-value pairs.
    /// </summary>
    public class JsonObjectDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the file.
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Gets or sets the data of the JSON object.
        /// The data is stored as key-value pairs in a dictionary, where the key is a string
        /// and the value is an object that can represent various data types.
        /// </summary>
        public Dictionary<string, object>? Data { get; set; }
    }
}
