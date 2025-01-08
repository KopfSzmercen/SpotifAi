using System.Text;
using Microsoft.Extensions.Options;
using OpenAI.Chat;

namespace SpotifAi.Ai;

internal sealed class OpenAiService(IOptions<OpenAiOptions> openAiConfiguration) : IAi
{
    public async Task<string> GetCompletionAsync(
        IReadOnlyList<Message> messages,
        AiCompletionSettings settings,
        CancellationToken cancellationToken
    )
    {
        var client = new ChatClient(GetModelName(settings.Model), openAiConfiguration.Value.ApiKey);

        ChatCompletionOptions options = new()
        {
            ResponseFormat = settings.JsonMode
                ? ChatResponseFormat.CreateJsonSchemaFormat(
                    "json",
                    BinaryData.FromBytes(Encoding.UTF8.GetBytes(settings.JsonSchema))
                )
                : ChatResponseFormat.CreateTextFormat()
        };

        List<ChatMessage> chatMessages = [];

        foreach (var message in messages)
        {
            if (message.Role == MessageRole.User)
            {
                chatMessages.Add(ChatMessage.CreateUserMessage(message.Text));
                continue;
            }

            chatMessages.Add(ChatMessage.CreateSystemMessage(message.Text));
        }

        var aiResponse = await client.CompleteChatAsync(chatMessages, options, cancellationToken);

        return aiResponse.Value.Content[0].Text;
    }

    private static string GetModelName(AiModel model)
    {
        return model switch
        {
            AiModel.Gpt4OMini => "gpt-4o-mini",
            AiModel.Gpt4o => "gpt-4o",
            _ => throw new ArgumentOutOfRangeException(nameof(model), model, null)
        };
    }
}