using Business_Layer.helpers;
using Business_Layer.services;
using Data_Layer.Data;
using Data_Layer.Interfaces;
using Data_Layer.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace API_Layer
{
    public static class Services
    {
        public static void RegisterServices(WebApplicationBuilder builder)
        {
            // DbContext
            if (builder.Environment.IsEnvironment("Testing"))
            {
                builder.Services.AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase("TestDb"));
            }
            else
            {
                builder.Services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            }

            //JWT Auth
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "")
                    ),

                    RoleClaimType = ClaimTypes.Role
                };
            });

            builder.Services.AddAuthorization();

            // Repositories + Services
            builder.Services.AddScoped<IUserRepository, UserRepo>();
            builder.Services.AddScoped<IProductRepository, ProductRepo>();
            builder.Services.AddScoped<AuthService>();
            builder.Services.AddScoped<TokenService>();
            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<UnitService>();
            builder.Services.AddScoped<ProductService>();
            builder.Services.AddScoped<CategoryService>();
            builder.Services.AddHttpContextAccessor();
        }
    }
}
