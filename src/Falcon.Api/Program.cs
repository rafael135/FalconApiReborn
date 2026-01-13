using System.Reflection;
using Falcon.Api.Extensions;
using Falcon.Api.Features.Competitions.Hubs;
using Falcon.Api.Features.Submissions.Consumers;
using Falcon.Api.Infrastructure;
using Falcon.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);


// Add infrastructure services (now includes JWT authentication configuration)
builder.Services.AddInfrastructure(builder.Configuration);

// Add MassTransit with API-specific consumers
builder.Services.AddApiMassTransit(x =>
{
    x.AddConsumer<SubmitExerciseResultConsumer>();
});

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
builder.Services.AddOpenApi();

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
        options.AddPreferredSecuritySchemes("JWT Bearer Token", "Token");

        options.AddAuthorizationCodeFlow(
            "JWT Bearer Token",
            flow =>
            {
                flow.Token = "CompetitionAuthToken";
                flow.SelectedScopes = new[] { "api.read", "api.write" };
            }
        );

        options.AddAuthorizationCodeFlow(
            "token",
            flow =>
            {
                flow.WithCredentialsLocation(CredentialsLocation.Body);
                flow.AddQueryParameter("token", "The JWT token");
                flow.SelectedScopes = new[] { "api.read", "api.write" };
            }
        );
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
