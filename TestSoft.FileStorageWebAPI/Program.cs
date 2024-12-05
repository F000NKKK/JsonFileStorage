using TestSoft.FileStorageLibrary.Services;
using TestSoft.FileStorageWebAPI.Services;

namespace TestSoft.FileStorageWebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Чтение относительного пути из конфигурации
            var relativeStorageDirectory = builder.Configuration.GetValue<string>("FileStorage:StorageDirectory");

            // Получаем абсолютный путь на основе текущей рабочей директории
            var storageDirectory = Path.Combine(Directory.GetCurrentDirectory(), relativeStorageDirectory);

            // Регистрация сервисов
            builder.Services.AddScoped<IJsonService, JsonService>();
            builder.Services.AddScoped<FileStorageService>(provider => new FileStorageService(storageDirectory));

            // Добавление контроллеров
            builder.Services.AddControllers();

            // Добавление Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Конфигурация HTTP pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Маршруты контроллеров
            app.MapControllers();

            app.Run();
        }
    }
}
