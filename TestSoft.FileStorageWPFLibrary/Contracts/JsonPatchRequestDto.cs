namespace TestSoft.FileStorageWPFLibrary.Contracts
{
    public class JsonPatchRequestDto
    {
        /// <summary>
        /// Список операций, которые нужно применить к JSON-объекту.
        /// </summary>
        public List<JsonPatchOperationDto> Operations { get; set; } = new();
    }

}
