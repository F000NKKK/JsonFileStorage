using TestSoft.FileStorageLibrary.Services;
using TestSoft.FileStorageWebAPI.Services;

namespace TestSoft.FileStorageWebAPI
{
    /// <summary>
    /// The entry point of the Web API application.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main method to run the application.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Read relative storage directory path from configuration
            /// <summary>
            /// Reading the relative file storage directory path from the application configuration.
            /// </summary>
            var relativeStorageDirectory = builder.Configuration.GetValue<string>("FileStorage:StorageDirectory");

            // Get the absolute path based on the current working directory
            /// <summary>
            /// Combining the current directory with the relative path to create the absolute path.
            /// </summary>
            var storageDirectory = Path.Combine(Directory.GetCurrentDirectory(), relativeStorageDirectory);

            // Register services with dependency injection container
            /// <summary>
            /// Registering the JSON service and file storage service for dependency injection.
            /// </summary>
            builder.Services.AddScoped<IJsonService, JsonService>();
            builder.Services.AddScoped<IFileSystem, FileSystem>();
            builder.Services.AddScoped<IFileStorageService, FileStorageService>(provider =>
                new FileStorageService(storageDirectory, provider.GetRequiredService<IFileSystem>()));

            // Add controllers to the service collection
            builder.Services.AddControllers();

            // Add Swagger for API documentation
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Map routes to controllers
            app.MapControllers();

            app.Run();
        }
    }
}
