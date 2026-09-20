using Backend.Application.Configurations;
using Backend.API.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Backend.Infrastructure.DependencyInjection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
// --------------------------------------------------
// Serilog
// --------------------------------------------------
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "Backend.API")
    .CreateLogger();

builder.Host.UseSerilog();

// --------------------------------------------------
// Services
// --------------------------------------------------
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --------------------------------------------------
// Infrastructure
// Database + Identity
// --------------------------------------------------
builder.Services.AddInfrastructure(builder.Configuration);

// --------------------------------------------------
// JWT Configuration
// --------------------------------------------------
var jwtSettings = builder.Configuration
    .GetSection("JwtSettings")
    .Get<JwtSettings>()
    ?? throw new InvalidOperationException(
        "JwtSettings configuration is missing.");

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

// --------------------------------------------------
// Authentication
// --------------------------------------------------
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.SecretKey)
            ),

            ClockSkew = TimeSpan.Zero
        };
    });

// --------------------------------------------------
// Authorization
// --------------------------------------------------
builder.Services.AddAuthorization();

var app = builder.Build();

// --------------------------------------------------
// Middleware Pipeline
// --------------------------------------------------

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();