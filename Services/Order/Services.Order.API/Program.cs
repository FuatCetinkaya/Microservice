
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
            var requireAuthorizePolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build(); // mutlaka bir subId(UserId) bekliyoruz. Cunku bu servis UserId ile �al���yor. Yani mutlaka Auth olmu� bir user olmas� gerekiyor.
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("Sub"); // Token ald���m�zda NameIdentifier olarak gelen bilginin Sub olarak gelmesini sa�lar. ��nk� Shared'da Sub olarak mapledik (UserClaim alanlar�nda NameIdentifier alan� Sub olarak gelmi� oldu)
            // Microservisleri Identity Bazl� koruma alt�na almak
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.Authority = builder.Configuration["IdentityServerURL"];
                options.Audience = "resource_order";  // Identity taraf�nda ekledi�miz resource
                options.RequireHttpsMetadata = false;
            });
            // +++

            // Add services to the container.

            builder.Services.AddControllers(opt =>
            {
                opt.Filters.Add(new AuthorizeFilter(requireAuthorizePolicy));  // +++ Mutlaka Auth olmu� bir kullan�c� gerekiyor
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