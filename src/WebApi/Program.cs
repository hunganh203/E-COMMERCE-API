using Application.DTOs.Configuration;
using Application.Interfaces.Authentication;
using Application.Interfaces.Service;
using AutoMapper;
using Cloud.Service;
using EfCore.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.Generation.Processors.Security;
using Shared;
using Shared.Mappings;
using System.IO.Compression;
using System.Net;
using System.Text;
using WebApi.Authentication;
using WebApi.Filter;
using WebApi.Models.Response;

const string defaultCorsPolicyName = "localhost";
var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
// Add services to the container.

#region Loading Configurations

#region AppConfiguration

var appConfiguration = new AppConfiguration();
configuration.Bind("App", appConfiguration);
builder.Services.AddSingleton(appConfiguration);

#endregion AppConfiguration

#region AWS

var awsConfiguration = new AwsConfiguration();
configuration.Bind("AwsConfig", awsConfiguration);
builder.Services.AddSingleton(awsConfiguration);

#endregion AWS

#region CloudService

var cloudServiceConfiguration = new CloudServiceConfiguration();
configuration.Bind("CloudService", cloudServiceConfiguration);
builder.Services.AddSingleton(cloudServiceConfiguration);

#endregion CloudService

#region Verification

var verificationConfiguration = new VerificationConfiguration();
configuration.Bind("Verification", verificationConfiguration);
builder.Services.AddSingleton(verificationConfiguration);

#endregion Verification

#region FileStorageSettingsOptions

var fileStorageSettingsOptions = new FileStorageSettingsOptions();
configuration.Bind("FileStorageSettings", fileStorageSettingsOptions);
builder.Services.AddSingleton(fileStorageSettingsOptions);

#endregion FileStorageSettingsOptions

#endregion Loading Configurations

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

var tokenKey = configuration.GetValue<string>("Authentication:JwtBearer:SecurityKey") ?? string.Empty;
var key = Encoding.ASCII.GetBytes(tokenKey);
builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
        };
        x.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                {
                    context.Response.Headers.Append("Token-Expired", "true");
                }
                return Task.CompletedTask;
            },
        };
    });

#pragma warning disable CS8604 // Possible null reference argument.
builder.Services.AddScoped<ITokenRefresher>(x =>
    new TokenRefresher(key, x.GetService<IJwtAuthenticationManager>()));

builder.Services.AddScoped<IRefreshTokenGenerator, RefreshTokenGenerator>();
builder.Services.AddScoped<IJwtAuthenticationManager, JwtAuthenticationManager>(x =>
    new JwtAuthenticationManager(tokenKey,
        x.GetService<IRefreshTokenGenerator>(),
        x.GetService<IUserService>(),
        x.GetService<ITokenRefreshService>(), configuration, x.GetService<ICustomerService>()));
builder.Services.AddEfCorePersistenceInfrastructure(configuration, LoggingOnStartup);
builder.Services.AddSharedInfrastructure(LoggingOnStartup);
builder.Services.AddCloudServiceInfrastructure(LoggingOnStartup);
#pragma warning restore CS8604 // Possible null reference argument.

#region Swagger

builder.Services.AddMvc();

builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
    );

builder.Services.AddSwaggerDocument(document =>
{
    document.Title = "ECommerce WebApi";
    document.OperationProcessors.Add(new OperationSecurityScopeProcessor("JWT token"));
    document.AddSecurity("JWT token", Enumerable.Empty<string>(), new OpenApiSecurityScheme
    {
        Type = OpenApiSecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Name = nameof(Authorization),
        BearerFormat = "JWT",
        Description = "Copy this into  the value field: \nBearer {my long token}"
    });

    document.OperationProcessors.Add(
        new AspNetCoreOperationSecurityScopeProcessor("Bearer"));
});

#endregion Swagger

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: defaultCorsPolicyName,
        policy =>
        {
            policy.WithOrigins(configuration["App:CorsOrigins"]
                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                .ToArray());

            policy.AllowAnyHeader();
            policy.AllowAnyMethod();
            policy.AllowCredentials();
        });
});

var mappingConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new CustomDtoMapper());
});
var mapper = mappingConfig.CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);

// Add Response compression services
builder.Services.AddResponseCompression(options =>
{
    options.Providers.Add<GzipCompressionProvider>();
    options.EnableForHttps = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseStaticFiles();

// Register the Swagger generator and the Swagger UI middlewares
app.UseOpenApi();
//app.UseSwaggerUi3();

app.UseSwaggerUI(options =>
{
    // specifying the Swagger JSON endpoint.
    options.SwaggerEndpoint("/swagger/v1/swagger.json",
    "ECommerce.WebApi v1");
    options.DisplayRequestDuration(); // Controls the display of the request duration (in milliseconds) for "Try it out" requests.
}); // URL: /swagger

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors(defaultCorsPolicyName); // Enable CORS!

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.Use(async (context, next) =>
{
    await next.Invoke();

    switch (context.Response.StatusCode)
    {
        //401 filter
        case 401:
            await context.Response.WriteAsJsonAsync(new AjaxResponse(new ErrorInfo
            {
                Code = context.Response.StatusCode,
                Message = "Current user did not login to the application!"
            }));
            break;

        case 403:
            await context.Response.WriteAsJsonAsync(new AjaxResponse(new ErrorInfo
            {
                Code = context.Response.StatusCode,
                Message = "You are not permission!"
            }));
            break;

        case 405:
            await context.Response.WriteAsJsonAsync(new AjaxResponse(new ErrorInfo
            {
                Code = context.Response.StatusCode,
                Message = "Method Not Allowed!"
            }));
            break;
    }
});

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

bool LoggingOnStartup(string message)
{
    return true;
}