using TestSoft.FileStorageLibrary.Services;
using TestSoft.FileStorageWebAPI.Services;

namespace TestSoft.FileStorageWebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ������ �������������� ���� �� ������������
            var relativeStorageDirectory = builder.Configuration.GetValue<string>("FileStorage:StorageDirectory");

            // �������� ���������� ���� �� ������ ������� ������� ����������
            var storageDirectory = Path.Combine(Directory.GetCurrentDirectory(), relativeStorageDirectory);

            // ����������� ��������
            builder.Services.AddScoped<IJsonService, JsonService>();
            builder.Services.AddScoped<FileStorageService>(provider => new FileStorageService(storageDirectory));

            // ���������� ������������
            builder.Services.AddControllers();

            // ���������� Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // ������������ HTTP pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // �������� ������������
            app.MapControllers();

            app.Run();
        }
    }
}
