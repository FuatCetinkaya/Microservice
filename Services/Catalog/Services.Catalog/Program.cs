
using Microsoft.Extensions.Options;
using Services.Catalog.Services;
using Services.Catalog.Settings;

namespace Services.Catalog
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAutoMapper(typeof(Program));
            builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("DatabaseSettings"));
            
            // interface yap�lmas�n�n sebebi herhangi bir class'da IOptions<DatabaseSetting> al�p  ctor'da options.Value dememek i�in yani
            // direk interface �zerinden gelmesi i�in yap�ld�.
            builder.Services.AddSingleton<IDatabaseSettings>(sp =>
            {
                var result = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
                return result;
            });


            builder.Services.AddSingleton<ICategoryService, CategoryService>();
            builder.Services.AddSingleton<ICourseService, CourseService>();


            builder.Services.AddControllers();



            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}