

using Domain.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.ServiceRegister
{
    //public class IdentityRegistration : IIdentityServicesRegistration
    //{
    //    public void AddServices(IServiceCollection services, IConfiguration configuration)
    //    {
    //        services.Configure<JwtTokenSettings>(configuration.GetSection("JwtSettings"));

    //        services.AddAuthentication(x =>
    //        {
    //            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    //            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    //            x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    //        }).AddJwtBearer(x =>
    //        {
    //            x.TokenValidationParameters = new TokenValidationParameters
    //            {
    //                ValidIssuer = configuration["JwtSettings:Issuer"],
    //                ValidAudience = configuration["JwtSettings:Issuer"],
    //                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"])),
    //                ValidateIssuer = true,
    //                ValidateAudience = true,
    //                ValidateLifetime = true,
    //                ValidateIssuerSigningKey = true
    //            };
    //            x.Events = new JwtBearerEvents
    //            {
    //                OnChallenge = context =>
    //                {
    //                    context.HandleResponse();
    //                    context.Response.StatusCode = 401;
    //                    context.Response.ContentType = "application/json";
    //                    var result = Result.Failure("Unauthorized access. Please log in with a valid token.");
    //                    var json = JsonSerializer.Serialize(result, new JsonSerializerOptions
    //                    {
    //                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    //                    });

    //                    return context.Response.WriteAsync(json);
    //                }
    //            };
    //        })
    //        .AddCookie(IdentityConstants.ApplicationScheme);

    //        services.AddAuthorization();
    //        services.AddIdentityCore<ApplicationUser>()
    //                .AddRoles<ApplicationRole>()
    //                .AddSignInManager<SignInManager<ApplicationUser>>()
    //                .AddEntityFrameworkStores<ApplicationDbContext>();
    //    }
    //}

    public class IdentityRegistration
    {
        public static void AddServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtTokenSettings>(configuration.GetSection("JwtSettings"));

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    ValidAudience = configuration["JwtSettings:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"])),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true
                };
                x.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        var result = Result.Failure("Unauthorized access. Please log in with a valid token.");
                        var json = JsonSerializer.Serialize(result, new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                        });

                        return context.Response.WriteAsync(json);
                    }
                };
            })
            .AddCookie(IdentityConstants.ApplicationScheme);

            services.AddAuthorization();
            services.AddIdentityCore<ApplicationUser>()
                    .AddRoles<ApplicationRole>()
                    .AddSignInManager<SignInManager<ApplicationUser>>()
                    .AddEntityFrameworkStores<ApplicationDbContext>();
        }
    }
}
