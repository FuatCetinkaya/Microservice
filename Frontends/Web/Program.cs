using FluentValidation.AspNetCore;
using IdentityModel.AspNetCore.AccessTokenManagement;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using Shared.Services;
using Web.Configuration;
using Web.Extensions;
using Web.Handler;
using Web.Helpers;
using Web.Services;
using Web.Services.Interfaces;
using Web.Validators;

namespace Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);






            //Option Pattern
            builder.Services.Configure<ServiceApiSettings>(builder.Configuration.GetSection("ServiceApiSettings"));
            builder.Services.Configure<ClientSettings>(builder.Configuration.GetSection("ClientSettings"));


            builder.Services.AddHttpContextAccessor();
            builder.Services.AddAccessTokenManagement();

            builder.Services.AddSingleton<PhotoHelper>();
             
            builder.Services.AddScoped<ISharedIdentityService, SharedIdentityService>();

            builder.Services.AddScoped<ResourceOwnerPasswordTokenHandler>();
            builder.Services.AddScoped<ClientCredentialTokenHandler>();


            builder.Services.AddHttpClientServices(builder.Configuration);//Extension
             



            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, opt =>
                {
                    opt.LoginPath = "/Auth/SignIn";
                    opt.ExpireTimeSpan = TimeSpan.FromDays(60);
                    opt.SlidingExpiration = true;
                    opt.Cookie.Name = "CookieName";
                });








            // Add services to the container.
            builder.Services.AddControllersWithViews().AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<CourseCreateInputValidator>());


             



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication(); //++++ 
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}