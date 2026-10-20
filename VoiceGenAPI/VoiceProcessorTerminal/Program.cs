using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.Versioning;
using System.Text;
using System.Text.Json;
using VoiceProcessor.Data;
using VoiceProcessorServer;

namespace VoiceProcessorTerminal;

[SupportedOSPlatform("windows")]
public static class Program
{
    private const string ServerFilePath = "VoiceProcessorServer.exe";
    
    public static async Task Main(string[] args)
    {
        RunServer();
        
        // Small delay to wait for server
        await Task.Delay(1000);

        using TcpClient client = new TcpClient();
        await client.ConnectAsync("127.0.0.1", ServerProgram.Port);
        await using NetworkStream stream = client.GetStream();
        using var reader = new StreamReader(stream, Encoding.UTF8);
        await using var writer = new StreamWriter(stream, Encoding.UTF8);
        writer.AutoFlush = true;

        Console.WriteLine("Server is now running in background");

        await Task.Delay(1000);
        
        Console.WriteLine();
        
        while (true)
        {
            Console.WriteLine("Enter your message to speak. Leave an empty input to stop the program.");
            var message = Console.ReadLine();
            if (string.IsNullOrEmpty(message))
            {
                await writer.WriteLineAsync("exit");
                return;
            }

            var input = new GenerationInput
            {
                VoiceId = "pda",
                Message = message,
                UseSsml = false
            };

            var id = Guid.NewGuid().ToString();

            var request = new VoiceLineRequest(input, id);

            await writer.WriteLineAsync(JsonSerializer.Serialize(request));
            var response = await reader.ReadLineAsync();
            Console.WriteLine("Response: " + response);
        }
    }

    private static void RunServer()
    {
        var thread = new Thread(() =>
        {
            var exePath = Path.GetFullPath(ServerFilePath);

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = exePath,
                    UseShellExecute = false
                }
            };

            process.Start();
        })
        {
            IsBackground = true
        };

        thread.Start();
    }
}