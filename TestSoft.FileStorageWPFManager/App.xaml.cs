using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net.Http;
using System.Windows;
using TestSoft.FileStorageWPFLibrary;
namespace TestSoft.FileStorageWPFManager
{
    public partial class App : Application
    {
        public FileStorageApiClient _fileStorageApiClient { get; private set; }
        private IConfiguration _configuration;

        public App()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            _configuration = builder.Build();

            var apiBaseUrl = _configuration["ApiSettings:BaseUrl"];

            if (string.IsNullOrEmpty(apiBaseUrl))
            {
                throw new InvalidOperationException("API Base URL is not configured in appsettings.json.");
            }

            var httpClient = new HttpClient();
            _fileStorageApiClient = new FileStorageApiClient(httpClient, apiBaseUrl);
        }

        public FileStorageApiClient FileStorageApiClient => _fileStorageApiClient;
    }
}
