using SpotifAi.Spotify;
using SpotifAi.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IClock, Clock>();

builder.Services.AddSpotify(builder.Configuration);


builder.Services.AddOpenApi();

var app = builder.Build();

app.UseSpotify();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/openapi/v1.json", "SpotifAi API v1"));

if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseHttpsRedirection();

app.Run();

namespace SpotifAi
{
    public class Program
    {
    }
}