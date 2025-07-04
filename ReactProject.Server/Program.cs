﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ReactProject.Server.Repositories;
using ReactProject.Server.Services;
using System.Security.Claims;
using System.Text;
using ReactProject.Server.Middleware;
namespace ReactProject.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // === Add Services ===

            // Controllers
            builder.Services.AddHttpClient("TranscriptionApi", client =>
            {
                client.BaseAddress = new Uri("https://python-api.internal.lemonwave-1b689309.westeurope.azurecontainerapps.io");   // ← Twój stały host
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            // Swagger / OpenAPI
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "ReactProject API", Version = "v1" });

                // Dodaj definicję schematu JWT
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Wprowadź token JWT poprzedzony słowem 'Bearer', np: **Bearer eyJhbGciOiJIUzI1...**",
                });


                options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
                    }
                });
            });

            // EF Core - Database
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

  

            // CORS
             builder.Services.AddCors(options =>
             {
                 options.AddPolicy("AllowFrontend",
                     policy =>
                     {
                         policy
                             .WithOrigins("https://frontend.lemonwave-1b689309.westeurope.azurecontainerapps.io","http://localhost:3000", "https://localhost:55071", "http://localhost:55071") // Twój frontend
                             .AllowAnyHeader()
                             .AllowAnyMethod();

                     });
             });




            // Custom model validation response
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                    new BadRequestObjectResult(context.ModelState);
            });
        
            // JWT Authentication
            var jwtKey = builder.Configuration["Jwt:Key"] ?? "powerfullkey";
            var keyBytes = Encoding.UTF8.GetBytes(jwtKey);
            
            builder.Services.AddScoped<ITokenValidatorService, TokenValidatorService>();
            builder.Services.AddScoped<JwtService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserTokenRepository, UserTokenRepository>();
            builder.Services.AddScoped<IFileRepository, FileRepository>();
            builder.Services.AddScoped<IFileService, FileService>();
            builder.Services.AddScoped<IUserStorageService, UserStorageService>();
            builder.Services.AddTransient<IFileService,FileService>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                    NameClaimType = ClaimTypes.Name,
                    RoleClaimType = ClaimTypes.Role,
      
                };
            });
            builder.Services.AddControllers();
            // === Build App ===
            var app = builder.Build();

            // === Configure Middleware ===

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger(); // <-- Dodajemy Swagger do środowiska deweloperskiego
                app.UseSwaggerUI(); // <-- Dodajemy UI Swaggera
            }
            app.Urls.Add("http://*:5000");
            if (!app.Environment.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }
            app.UseCors("AllowFrontend");
            
             // CORS
            app.UseAuthentication();// JWT
            app.UseMiddleware<TokenRevocationMiddleware>();
            app.UseAuthorization();    // Role-based or claims-based auth

            app.UseDefaultFiles();     // index.html, etc.
            app.MapStaticAssets();     // wwwroot

            app.MapControllers();      // API endpoints
            app.MapFallbackToFile("/index.html"); // SPA fallback
            app.UseStaticFiles();
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.Migrate(); // lub EnsureCreated() jeśli nie używasz migracji
                   
            }
            app.Run();
            
        }
    }
}
