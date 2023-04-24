using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace Gateway
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddJsonFile($"configuration.{builder.Environment.EnvironmentName.ToLower()}.json");   //++


            builder.Services.AddAuthentication().AddJwtBearer("GatewayAuthenticationSchema", options =>
            {
                options.Authority = builder.Configuration["IdentityServerURL"];
                options.Audience = "resource_gateway";
                options.RequireHttpsMetadata = false;
            });

            builder.Services.AddOcelot();   // ++

            var app = builder.Build();

            await app.UseOcelot();    // ++

            app.Run();
        }
    }
}