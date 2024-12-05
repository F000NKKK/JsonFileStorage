namespace TestSoft.FileStorageLibrary.Contracts
{
    public class FileDataDto
    {
        public Guid Id { get; set; }
        public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();
    }
}

