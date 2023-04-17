
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Authorization;
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
            
            // interface yapýlmasýnýn sebebi herhangi bir class'da IOptions<DatabaseSetting> alýp  ctor'da options.Value dememek için yani
            // direk interface üzerinden gelmesi için yapýldý.
            builder.Services.AddSingleton<IDatabaseSettings>(sp =>
            {
                var result = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
                return result;
            });


            builder.Services.AddSingleton<ICategoryService, CategoryService>();
            builder.Services.AddSingleton<ICourseService, CourseService>();


            builder.Services.AddControllers(opt=>
            {
                opt.Filters.Add(new AuthorizeFilter()); // Tüm api'lerin tepesine Authorize yazmamak için
            });


            // Microservisleri Identity Bazlý koruma altýna almak
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.Authority = builder.Configuration["IdentityServerURL"];
                options.Audience = "resource_catalog";  // Identity tarafýnda eklediðmiz resource
                options.RequireHttpsMetadata= false;
            });



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

            app.UseAuthentication();
   
            app.MapControllers();

            app.Run();
        }
    }
}