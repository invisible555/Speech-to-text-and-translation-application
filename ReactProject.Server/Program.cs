using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ReactProject.Server.Repositories;
using ReactProject.Server.Services;
using System.Text;

namespace ReactProject.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // === Add Services ===
       
            // Controllers
      

            // Swagger / OpenAPI
            builder.Services.AddSwaggerGen(); // <-- To dodaj, jeœli jeszcze tego nie masz
          
            // EF Core - Database
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.WithOrigins("https://localhost:55071") // frontend URL
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

            builder.Services.AddScoped<JwtService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserTokenRepository, UserTokenRepository>();


            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
                };
            });
            builder.Services.AddControllers();
            // === Build App ===
            var app = builder.Build();

            // === Configure Middleware ===

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger(); // <-- Dodajemy Swagger do œrodowiska deweloperskiego
                app.UseSwaggerUI(); // <-- Dodajemy UI Swaggera
            }

            app.UseHttpsRedirection();

            app.UseCors();             // CORS
            app.UseAuthentication();   // JWT
            app.UseAuthorization();    // Role-based or claims-based auth

            app.UseDefaultFiles();     // index.html, etc.
            app.MapStaticAssets();     // wwwroot

            app.MapControllers();      // API endpoints
            app.MapFallbackToFile("/index.html"); // SPA fallback
            app.UseStaticFiles();

            app.Run();
        }
    }
}
