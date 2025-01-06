namespace SpotifAi.Spotify.AuthorizationStateManager;

public interface IAuthorizationStateManager
{
    string GenerateRandomStateValue();

    Task StoreStateValueAsync(string state, CancellationToken cancellationToken);

    Task<bool> ValidateStateValueAsync(string state, CancellationToken cancellationToken);

    Task InvalidateStateValueAsync(string state, CancellationToken cancellationToken);
}