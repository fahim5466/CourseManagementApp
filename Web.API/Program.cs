using Web.API.Helpers;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Microsoft.Extensions.Options;
using Web.API.Filters;
using Application.Interfaces;
using Web.API.Middlewares;

namespace Web.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog((context, loggerConfiguration) =>
            {
                loggerConfiguration.ReadFrom.Configuration(context.Configuration);
            });

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<IHttpHelper, HttpHelper>();

            builder.Services.AddInfrastructure(builder.Configuration);

            builder.Services.AddControllers(
                                options => options.Filters.Add<PreprocessRequestDtoFilter>()
                             )
                            .AddJsonOptions(options =>
                             {
                                 options.AllowInputFormatterExceptionMessages = false;
                             });

            builder.Services.AddOpenApi();

            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
                


            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.ApplyMigrations();
            };

            app.UseHttpsRedirection();

            app.UseMiddleware<GlobalExceptionHandler>();

            app.UseSerilogRequestLogging();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            await app.RunAsync();
        }
    }
}
