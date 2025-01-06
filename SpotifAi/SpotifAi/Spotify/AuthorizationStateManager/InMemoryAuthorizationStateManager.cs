using System.Collections.Concurrent;
using SpotifAi.Utils;

namespace SpotifAi.Spotify.AuthorizationStateManager;

internal sealed class InMemoryAuthorizationStateManager(IClock clock) : IAuthorizationStateManager
{
    private readonly ConcurrentDictionary<string, DateTimeOffset> _stateValues = [];

    public string GenerateRandomStateValue()
    {
        var state = Guid.NewGuid().ToString();
        return state;
    }

    public Task StoreStateValueAsync(string state, CancellationToken cancellationToken)
    {
        _stateValues.TryAdd(state, clock.Now);
        return Task.CompletedTask;
    }

    public Task<bool> ValidateStateValueAsync(string state, CancellationToken cancellationToken)
    {
        return Task.FromResult(_stateValues.ContainsKey(state));
    }

    public Task InvalidateStateValueAsync(string state, CancellationToken cancellationToken)
    {
        _stateValues.TryRemove(state, out _);
        return Task.CompletedTask;
    }
}