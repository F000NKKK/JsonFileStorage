namespace TestSoft.FileStorageWebAPI.Contracts
{
    public class JsonObjectDto
    {
        public Dictionary<string, object>? Data { get; set; }  // теперь может быть null
    }

}
