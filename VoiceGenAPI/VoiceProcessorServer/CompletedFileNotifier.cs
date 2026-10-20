using System.Net.Http.Json;
using VoiceProcessor.Data;

namespace VoiceProcessorServer;

public class CompletedFileNotifier(HttpClient httpClient, int port, string path)
{
    public async Task NotifyFileReadyAsync(GenerationResult result)
    {
        HttpResponseMessage response;
        try
        {
            response = await httpClient.PostAsJsonAsync($"http://localhost:{port}/{path}", result);
        }
        catch (Exception e)
        {
            Console.WriteLine("ERROR: " + e.Message);
            return;
        }
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Notification response: {responseContent}");
    }
}