using System.Runtime.CompilerServices;
using SpotifAi.Ai;
using SpotifAi.Auth;
using SpotifAi.Auth.RequestContext;
using SpotifAi.Persistence;
using SpotifAi.Scrapping;
using SpotifAi.Spotify;
using SpotifAi.Users;
using SpotifAi.Utils;

[assembly: InternalsVisibleTo("SpotifAi.Tests.Integration")]

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IClock, Clock>();
builder.Services.AddRequestContext();

builder.Services.AddScrappingModule(builder.Configuration);
builder.Services.AddPersistenceModule(builder.Configuration);
builder.Services.AddSpotifyModule(builder.Configuration);
builder.Services.AddUsersModule(builder.Configuration);
builder.Services.AddAiModule(builder.Configuration);

builder.Services.AddOpenApi();

builder.Services.AddAuth();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        b =>
        {
            b.WithOrigins("http://localhost:3000");
            b.AllowAnyOrigin();
            b.AllowAnyMethod();
            b.AllowAnyHeader();
        }
    );
});

var app = builder.Build();

await app.UsePersistenceModuleAsync();
app.UseSpotifyModule();
app.UseUsersModule();
app.UseScrappingModule();

app.UseSwaggerUI(c => c.SwaggerEndpoint("/openapi/v1.json", "SpotifAi API v1"));

if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.UseRequestContext();

app.UseCors();

app.Run();


namespace SpotifAi
{
    public class Program
    {
    }
}