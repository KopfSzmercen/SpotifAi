using System.Text;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace SpotifAi.Tests.Integration.TestSpotiftApi;

public class TestSpotifyApi : IAsyncLifetime
{
    private WireMockServer _wireMockServer = null!;
    public string BaseAddress = null!;

    public Task InitializeAsync()
    {
        _wireMockServer = WireMockServer.Start();
        BaseAddress = _wireMockServer.Urls[0];
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        _wireMockServer.Stop();
        return Task.CompletedTask;
    }

    //Setup 200 get access token response

    public void SetupGetAccessTokenResponse(
        string code,
        string redirectUri,
        string grantType,
        string clientId,
        string clientSecret
    )
    {
        _wireMockServer
            .Given(Request.Create()
                .WithPath("/api/token")
                .UsingPost()
                .WithHeader("Content-Type", "application/x-www-form-urlencoded")
                .WithBody(new FormUrlEncodedMatcher([
                        "code=" + code,
                        "redirect_uri=" + redirectUri,
                        "grant_type=" + grantType
                    ])
                )
                .WithHeader("Authorization",
                    new ExactMatcher("Basic " +
                                     Convert.ToBase64String(Encoding.UTF8.GetBytes(clientId + ":" + clientSecret))))
            )
            .RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithBody(
                        """
                        {
                            "access_token":"access_token",
                            "token_type":"Bearer",
                            "expires_in":3600,
                            "refresh_token":"refresh_token",
                            "scope":"user-read-private user-read-email"
                        }
                        """)
            );
    }

    public void SetupRefreshAccessTokenResponse(
        string refreshToken,
        string clientId,
        string clientSecret
    )
    {
        _wireMockServer
            .Given(Request.Create()
                .WithPath("/api/token")
                .UsingPost()
                .WithHeader("Content-Type", "application/x-www-form-urlencoded")
                .WithBody(new FormUrlEncodedMatcher([
                    "grant_type=refresh_token",
                    "refresh_token=" + refreshToken,
                    "client_id=" + clientId
                ]))
                .WithHeader("Authorization",
                    new ExactMatcher("Basic " +
                                     Convert.ToBase64String(Encoding.UTF8.GetBytes(clientId + ":" + clientSecret))))
            )
            .RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithBody(
                        """
                        {
                            "access_token":"access_token",
                            "token_type":"Bearer",
                            "expires_in":3600,
                            "scope":"user-read-private user-read-email"
                        }
                        """)
            );
    }
}