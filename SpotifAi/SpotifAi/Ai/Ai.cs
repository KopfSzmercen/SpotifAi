﻿namespace SpotifAi.Ai;

public enum AiModel
{
    Gpt4OMini,
    Gpt4o
}

public enum MessageRole
{
    User,
    System
}

public sealed record Message(MessageRole Role, string Text);

public sealed record AiCompletionSettings
{
    public required AiModel Model { get; init; }

    public required bool JsonMode { get; init; }
}

public interface IAi
{
    Task<string> GetCompletionAsync(
        IReadOnlyList<Message> messages,
        AiCompletionSettings settings,
        CancellationToken cancellationToken
    );
}