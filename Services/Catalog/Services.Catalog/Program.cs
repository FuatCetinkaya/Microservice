
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Services.Catalog.Dtos;
using Services.Catalog.Services;
using Services.Catalog.Settings;
using System.Reflection;

namespace Services.Catalog;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
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


        builder.Services.AddControllers(opt=>
        {
            opt.Filters.Add(new AuthorizeFilter()); // T�m api'lerin tepesine Authorize yazmamak i�in
        });


        // Microservisleri Identity Bazl� koruma alt�na almak
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.Authority = builder.Configuration["IdentityServerURL"];
            options.Audience = "resource_catalog";  // Identity taraf�nda ekledi�miz resource
            options.RequireHttpsMetadata= false;
        });



        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Seed data //+++
        using (var scope = app.Services.CreateScope())
        {
            var serviceProvider = scope.ServiceProvider;

            var categoryService = serviceProvider.GetRequiredService<ICategoryService>();

            if (!categoryService.GetAllAsync().Result.Data.Any())
            {
                categoryService.CreateAsync(new CategoryCreateDto { Name = "Asp.net Core Kursu" }).Wait();
                categoryService.CreateAsync(new CategoryCreateDto { Name = "Asp.net Core API Kursu" }).Wait();
            }
        }
        // Seed data //+++

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