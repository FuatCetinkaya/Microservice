
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Services.Order.Infrastructure;
using Shared.Services;
using System.IdentityModel.Tokens.Jwt;

namespace Services.Order.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);




            // +++
            var requireAuthorizePolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build(); // mutlaka bir subId(UserId) bekliyoruz. Cunku bu servis UserId ile çalýþýyor. Yani mutlaka Auth olmuþ bir user olmasý gerekiyor.
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("Sub"); // Token aldýðýmýzda NameIdentifier olarak gelen bilginin Sub olarak gelmesini saðlar. Çünkü Shared'da Sub olarak mapledik (UserClaim alanlarýnda NameIdentifier alaný Sub olarak gelmiþ oldu)
            // Microservisleri Identity Bazlý koruma altýna almak
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.Authority = builder.Configuration["IdentityServerURL"];
                options.Audience = "resource_order";  // Identity tarafýnda eklediðmiz resource
                options.RequireHttpsMetadata = false;
            });
            // +++

            // Add services to the container.

            builder.Services.AddControllers(opt =>
            {
                opt.Filters.Add(new AuthorizeFilter(requireAuthorizePolicy));  // +++ Mutlaka Auth olmuþ bir kullanýcý gerekiyor
            });









            builder.Services.AddDbContext<OrderDbContext>(opt =>
            {
                opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), configure =>
                {
                    configure.MigrationsAssembly("FreeCourse.Services.Order.Infrastructure");
                });
            });


            builder.Services.AddMediatR(typeof(Services.Order.Application.Handlers.CreateOrderCommandHandler).Assembly);
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<ISharedIdentityService, SharedIdentityService>();





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

            app.UseAuthentication(); // +++

            app.MapControllers();

            app.Run();
        }
    }
}