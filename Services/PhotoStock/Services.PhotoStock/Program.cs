
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace Services.PhotoStock
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Microservisleri Identity Bazlý koruma altýna almak
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.Authority = builder.Configuration["IdentityServerURL"];
                options.Audience = "resource_photo_stock";  // Identity tarafýnda eklediðmiz resource
                options.RequireHttpsMetadata = false;
            });



            // Add services to the container.

            builder.Services.AddControllers(opt =>
            {
                opt.Filters.Add(new AuthorizeFilter()); // tüm controller'lara Auth Attribute'unu koymaktan kurtarýr
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

            ///
            app.UseStaticFiles();
            app.UseAuthentication();
            ///

            app.MapControllers();

            app.Run();
        }
    }
}