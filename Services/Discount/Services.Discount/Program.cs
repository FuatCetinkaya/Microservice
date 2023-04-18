
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Services.Discount.Services;
using Shared.Services;
using System.IdentityModel.Tokens.Jwt;

namespace Services.Discount
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);




            //Shared'Daki User ID'yi kullanabilmek için
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<ISharedIdentityService, SharedIdentityService>(); // Sharedden User Id almak için


            //IDiscountService
            builder.Services.AddScoped<IDiscountService, DiscountService>();


            // MikroServisi koruma altýna alma

            var requireAuthorizePolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build(); // mutlaka bir subId(UserId) bekliyoruz. Cunku bu servis UserId ile çalýþýyor. Yani mutlaka Auth olmuþ bir user olmasý gerekiyor.

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("Sub"); // Token aldýðýmýzda NameIdentifier olarak gelen bilginin Sub olarak gelmesini saðlar. Çünkü Shared'da Sub olarak mapledik (UserClaim alanlarýnda NameIdentifier alaný Sub olarak gelmiþ oldu)

            // Microservisleri Identity Bazlý koruma altýna almak
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.Authority = builder.Configuration["IdentityServerURL"];
                options.Audience = "resource_discount";  // Identity tarafýnda eklediðmiz resource
                options.RequireHttpsMetadata = false;
            });


            builder.Services.AddControllers(opt =>
            {
                opt.Filters.Add(new AuthorizeFilter(requireAuthorizePolicy));  // +++ Mutlaka Auth olmuþ bir kullanýcý gerekiyor
            });
            // MikroServisi koruma altýna alma







            // Add services to the container.

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
            app.UseAuthentication(); //


            app.MapControllers();

            app.Run();
        }
    }
}