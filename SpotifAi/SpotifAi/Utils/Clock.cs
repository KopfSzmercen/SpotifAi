namespace SpotifAi.Utils;

public interface IClock
{
    DateTimeOffset Now { get; }
}

public sealed class Clock : IClock
{
    public DateTimeOffset Now => DateTimeOffset.Now;
}