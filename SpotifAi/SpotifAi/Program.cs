using System.Runtime.CompilerServices;
using SpotifAi.Auth;
using SpotifAi.Auth.RequestContext;
using SpotifAi.Persistence;
using SpotifAi.Spotify;
using SpotifAi.Users;
using SpotifAi.Utils;

[assembly: InternalsVisibleTo("SpotifAi.Tests.Integration")]

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IClock, Clock>();
builder.Services.AddRequestContext();

builder.Services.AddPersistenceModule(builder.Configuration);
builder.Services.AddSpotifyModule(builder.Configuration);
builder.Services.AddUsersModule(builder.Configuration);


builder.Services.AddOpenApi();

builder.Services.AddAuth();

var app = builder.Build();

await app.UsePersistenceModuleAsync();
app.UseSpotifyModule();
app.UseUsersModule();

app.UseSwaggerUI(c => c.SwaggerEndpoint("/openapi/v1.json", "SpotifAi API v1"));

if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.UseRequestContext();


app.Run();

namespace SpotifAi
{
    public class Program
    {
    }
}