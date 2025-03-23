using Web.API.Helpers;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Web.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddInfrastructure(builder.Configuration); ;

            // Add services to the container.

            builder.Services.AddControllers()
                            .AddJsonOptions(options =>
                             {
                                 options.AllowInputFormatterExceptionMessages = false;
                             });

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
                


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.ApplyMigrations();
            }


            app.MapGet("/", () => Results.Ok(new
            {
                message = "Hello from the course management app!"
            }));


            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            await app.RunAsync();
        }
    }
}
