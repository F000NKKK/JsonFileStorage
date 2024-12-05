namespace TestSoft.FileStorageWPFLibrary.Contracts
{
    public class JsonPatchOperationDto
    {
        /// <summary>
        /// Тип операции: add, replace, remove.
        /// </summary>
        public required string Op { get; set; }

        /// <summary>
        /// Путь к элементу в JSON, например: /property/nestedProperty.
        /// </summary>
        public required string Path { get; set; }

        /// <summary>
        /// Объект, который добавляется или заменяется (используется для add/replace).
        /// </summary>
        public object? Value { get; set; }
    }

}
