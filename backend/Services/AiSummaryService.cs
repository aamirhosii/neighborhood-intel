using OpenAI;
using OpenAI.Chat;
using NeighborhoodIntel.Api.Models;

namespace NeighborhoodIntel.Api.Services;

public class AiSummaryService(IConfiguration config)
{
    private readonly ChatClient _client = new(
        model: "gpt-4o-mini",
        apiKey: config["OpenAI:ApiKey"] ?? throw new InvalidOperationException("OpenAI:ApiKey not configured")
    );

    public async Task<string> SummarizeAsync(PlaceCounts counts, double score, string address)
    {
        var prompt =
            $"""
            You are a professional real estate advisor. A buyer is evaluating a neighbourhood.

            Location: {address}
            Neighborhood score: {score}/100

            Amenities found nearby:
            - Schools: {counts.Schools}
            - Parks: {counts.Parks}
            - Grocery stores: {counts.Grocery}
            - Transit stops: {counts.Transit}
            - Restaurants: {counts.Restaurants}

            Write a concise 3-paragraph evaluation (max 150 words total):
            1. Overall impression based on the score and amenity mix.
            2. Key strengths.
            3. One or two areas for improvement or things the buyer should investigate further.

            Be direct, practical, and avoid filler phrases.
            """;

        var messages = new List<ChatMessage>
        {
            new UserChatMessage(prompt)
        };

        var result = await _client.CompleteChatAsync(messages);
        return result.Value.Content[0].Text ?? "Unable to generate summary.";
    }
}
