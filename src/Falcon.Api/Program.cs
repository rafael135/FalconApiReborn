using System.Reflection;
using Falcon.Api.Extensions;
using Falcon.Api.Features.Competitions.Hubs;
using Falcon.Api.Features.Submissions.Consumers;
using Falcon.Api.Infrastructure;
using Falcon.Infrastructure;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add infrastructure services (now includes JWT authentication configuration)
builder.Services.AddInfrastructure(builder.Configuration);

var configuration = builder.Configuration;

// Add MassTransit with API-specific consumers
builder.Services.AddApiMassTransit(
    x =>
    {
        x.AddConsumer<SubmitExerciseResultConsumer>();
    },
    configuration
);

// Add MediatR services - Searchs for handlers in the current assembly
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())
);

// Add services to the container.

builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// SignalR
builder.Services.AddSignalR();

// CORS for SignalR
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowFrontend",
        policy =>
        {
            policy
                .WithOrigins("http://localhost:3000", "http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        }
    );
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_0;

    options.AddDocumentTransformer(
        (document, context, cancellationToken) =>
        {
            document.Components ??= new();

            OpenApiSecurityScheme cookieSecurityScheme = new OpenApiSecurityScheme
            {
                Name = "CompetitionAuthToken",
                Description = "Enter your JWT Bearer token in the format **Bearer &lt;token&gt;**",
                In = ParameterLocation.Cookie,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
            };

            document.Components.SecuritySchemes!.Add("Bearer", cookieSecurityScheme);

            OpenApiSecurityScheme querySecurityScheme = new OpenApiSecurityScheme
            {
                Name = "token",
                Description = "Enter your JWT Bearer token in the format **&lt;token&gt;**",
                In = ParameterLocation.Query,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
            };

            document.Components.SecuritySchemes!.Add("token", querySecurityScheme);

            return Task.CompletedTask;
        }
    );
});

var app = builder.Build();

app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Falcon API - Documentation");
        options.WithTheme(ScalarTheme.Purple);
        options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });

    app.MapGet("/", () => Results.Redirect("/scalar/v1")).ExcludeFromDescription();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<CompetitionHub>("/hubs/competition");

// Map all endpoints
// app.MapAllEndpoints();
app.MapEndpoints();

app.Run();
