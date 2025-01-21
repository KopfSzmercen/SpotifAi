using SpotifAi.Ai.Agent;

namespace SpotifAi.Ai.Tools;

public abstract class AgentTool(Tool name, string description)
{
    public Tool Name { get; set; } = name;

    public string Description { get; set; } = description;


    public bool IsApplicable(Tool tool)
    {
        return Name == tool;
    }

    public abstract Task<string> ExecuteAsync(string parameters, CancellationToken cancellationToken);
}