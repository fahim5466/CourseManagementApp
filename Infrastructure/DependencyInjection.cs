using Application;
using Application.Interfaces;
using Application.Services;
using Domain.Repositories;
using Infrastructure.Database;
using Infrastructure.Notifications;
using Infrastructure.Repositories;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddDatabase(configuration)
                           .AddRepositories()
                           .AddApplicationServices()
                           .AddSecurity(configuration)
                           .AddNotifications(configuration);
        }

        private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            string? connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDbContext>(
                options => options.UseNpgsql(connectionString)
                                  .LogTo(Console.WriteLine, LogLevel.Information)
                                  .UseSnakeCaseNamingConvention());

            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IClassRepository, ClassRepository>();
            services.AddScoped<ICourseRepository, CourseRepository>();

            return services;
        }

        private static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IClassService, ClassService>();
            services.AddScoped<ICourseService, CourseService>();

            return services;
        }

        private static IServiceCollection AddSecurity(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!)),
                    ValidIssuer = configuration["Jwt:Issuer"]!,
                    ValidAudience = configuration["Jwt:Audience"]!,
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies["AccessToken"];
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddSingleton<ICryptoHasher, CryptoHasher>();
            services.AddSingleton<ISecurityTokenProvider, SecurityTokenProvider>();

            return services;
        }

        private static IServiceCollection AddNotifications(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IEmailService, EmailService>();

            services.AddFluentEmail(configuration["Email:SenderEmail"], configuration["Email:Sender"])
                    .AddSmtpSender(configuration["Email:Host"], configuration.GetValue<int>("Email:Port"));
            return services;
        }
    }
}
