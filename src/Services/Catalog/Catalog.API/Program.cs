using System.Text;
using System.Text.Json;

using Catalog.API.Mapping;
using Catalog.API.Repositories;
using Catalog.API.Settings;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

using PulseDelivery.Shared.Configurations;
using PulseDelivery.Shared.Exceptions;
using PulseDelivery.Shared.DTOs;
using PulseDelivery.Shared.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// AutoMapper
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<GeneralMapping>();
});

// MongoDB Settings
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

// Repository
builder.Services.AddScoped<IRestaurantRepository, RestaurantRepository>();

// JWT Configuration
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

var jwtSettings = builder.Configuration
    .GetSection("JwtSettings")
    .Get<JwtSettings>()!;

// JWT Authentication
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
                Encoding.UTF8.GetBytes(jwtSettings.SecurityKey)
            ),

            // No expiration tolerance
            ClockSkew = TimeSpan.Zero
        };

        // JWT Error Responses
        options.Events = new JwtBearerEvents
        {
            // 401 Unauthorized
            OnChallenge = async context =>
            {
                context.HandleResponse();

                context.Response.StatusCode =
                    StatusCodes.Status401Unauthorized;

                context.Response.ContentType = "application/json";

                var result = JsonSerializer.Serialize(
                    ResponseDto<NoContent>.Fail(
                        "Authentication failed. Please provide a valid token.",
                        401));

                await context.Response.WriteAsync(result);
            },

            // 403 Forbidden
            OnForbidden = async context =>
            {
                context.Response.StatusCode =
                    StatusCodes.Status403Forbidden;

                context.Response.ContentType = "application/json";

                var result = JsonSerializer.Serialize(
                    ResponseDto<NoContent>.Fail(
                        "You are not authorized to perform this action.",
                        403));

                await context.Response.WriteAsync(result);
            }
        };
    });

// Swagger
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",

        Description =
            "Enter 'Bearer' followed by your JWT token.\n\n" +
            "Example: Bearer eyJhbGciOiJIUzI1NiIs...",

        In = ParameterLocation.Header,

        Type = SecuritySchemeType.Http,

        Scheme = "bearer",

        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
});

// Authorization Policies
builder.Services.AddAuthorization(options =>
{
    // Platform owner only (e.g., deleting or creating restaurants)
    options.AddPolicy(Permissions.SystemAdmin, policy =>
        policy.RequireClaim("Permission", Permissions.SystemAdmin));

    // Catalog/Menu write permission (restaurant owners, managers, etc.)
    options.AddPolicy(Permissions.CatalogWrite, policy =>
        policy.RequireClaim("Permission", Permissions.CatalogWrite));

    // Catalog/Menu read permission (customers, couriers, employees, etc.)
    options.AddPolicy(Permissions.CatalogRead, policy =>
        policy.RequireClaim("Permission", Permissions.CatalogRead));
});

var app = builder.Build();

// Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTPS Redirect
app.UseHttpsRedirection();
// Global Exception Handler
app.UseCustomException();

// Authentication
app.UseAuthentication();

// Authorization
app.UseAuthorization();

// Controllers
app.MapControllers();

app.Run();
