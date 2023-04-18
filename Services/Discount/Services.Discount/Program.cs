
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




            //Shared'Daki User ID'yi kullanabilmek i�in
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<ISharedIdentityService, SharedIdentityService>(); // Sharedden User Id almak i�in


            //IDiscountService
            builder.Services.AddScoped<IDiscountService, DiscountService>();


            // MikroServisi koruma alt�na alma

            var requireAuthorizePolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build(); // mutlaka bir subId(UserId) bekliyoruz. Cunku bu servis UserId ile �al���yor. Yani mutlaka Auth olmu� bir user olmas� gerekiyor.

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("Sub"); // Token ald���m�zda NameIdentifier olarak gelen bilginin Sub olarak gelmesini sa�lar. ��nk� Shared'da Sub olarak mapledik (UserClaim alanlar�nda NameIdentifier alan� Sub olarak gelmi� oldu)

            // Microservisleri Identity Bazl� koruma alt�na almak
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.Authority = builder.Configuration["IdentityServerURL"];
                options.Audience = "resource_discount";  // Identity taraf�nda ekledi�miz resource
                options.RequireHttpsMetadata = false;
            });


            builder.Services.AddControllers(opt =>
            {
                opt.Filters.Add(new AuthorizeFilter(requireAuthorizePolicy));  // +++ Mutlaka Auth olmu� bir kullan�c� gerekiyor
            });
            // MikroServisi koruma alt�na alma







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