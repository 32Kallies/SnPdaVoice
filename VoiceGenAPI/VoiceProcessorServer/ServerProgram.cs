using System.Net;
using System.Net.Sockets;
using System.Runtime.Versioning;
using System.Text;
using System.Text.Json;
using VoiceProcessor.Utilities;

namespace VoiceProcessorServer;

[SupportedOSPlatform("windows")]
public static class ServerProgram
{
    private static readonly List<VoiceLineQueue> Queues = [];
    private static readonly List<VoiceLineQueue> LongVoiceLineQueues = [];

    public const int Port = 8765;

    private const int NormalQueueCount = 1;
    private const int LongVoiceLineQueueCount = 0;
    private const int LongVoiceLineMinCharacterCutOff = 400;

    private static TcpListener? _listener;
    private static StreamReader? _reader;
    private static StreamWriter? _writer;

    public static bool IsReady { get; private set; }

    private static TimeSpan FileLifetime { get; } = TimeSpan.FromDays(1);
    private static TimeSpan FileDeletionAttemptsInterval { get; } = TimeSpan.FromHours(1);

    internal static readonly Telemetry Telemetry = new();

    public static async Task Main(string[] args)
    {
        _listener = new TcpListener(IPAddress.Loopback, Port);
        _listener.Start();
        Console.WriteLine($"Listening on port {Port}...");

        var voices = VoiceLoadingUtils.LoadAllVoices();

        if (voices.Count == 0)
        {
            Console.WriteLine("No voices found! Press enter to exit...");
            Console.ReadLine();
            return;
        }

        for (int i = 0; i < NormalQueueCount; i++)
        {
            var queue = new VoiceLineQueue("NormalQueue" + i, voices);
            Queues.Add(queue);
            queue.StartProcessingEntries();
        }

        for (int i = 0; i < LongVoiceLineQueueCount; i++)
        {
            var queue = new VoiceLineQueue("LongLineQueue" + i, voices);
            LongVoiceLineQueues.Add(queue);
            queue.StartProcessingEntries();
        }

        Task.Run(ClearUnusedFiles);

        Console.WriteLine($"Voice systems initialized. {voices.Count} voices found.");
        Console.WriteLine("Starting server...");

        IsReady = true;

        Telemetry.LogServerStart();

        while (true)
        {
            Console.WriteLine("Waiting for a new client...");
            using TcpClient client = await _listener.AcceptTcpClientAsync();
            Console.WriteLine("Client connected!");

            await HandleClient(client);
            Console.WriteLine("Client disconnected.");
        }
    }

    private static async Task HandleClient(TcpClient client)
    {
        await using NetworkStream stream = client.GetStream();
        using var reader = new StreamReader(stream, Encoding.UTF8);
        await using var writer = new StreamWriter(stream, Encoding.UTF8);
        writer.AutoFlush = true;

        string? line;
        while ((line = await reader.ReadLineAsync()) != null)
        {
            if (line == "exit")
                break;

            Console.WriteLine($"Received: {line}");

            VoiceLineRequest? request;

            try
            {
                request = JsonSerializer.Deserialize<VoiceLineRequest>(line);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e);
                Telemetry.LogError($"Exception thrown while processing received line '{line}': " + e);
                await writer.WriteLineAsync("Exception thrown!");
                continue;
            }

            QueueRequest(request);
            await writer.WriteLineAsync("Enqueued request successfully!");
        }
    }

    public static void QueueRequest(VoiceLineRequest request)
    {
        GetBestQueueForInput(request).EnqueueVoiceLineRequest(request);
        Telemetry.LogReceivedRequest(request);
    }

    private static async Task ClearUnusedFiles()
    {
        while (true)
        {
            CleanUpUtils.ClearAllOldFiles(FileLifetime);
            await Task.Delay(FileDeletionAttemptsInterval);
        }
    }

    private static VoiceLineQueue GetBestQueueForInput(VoiceLineRequest request)
    {
        if (NormalQueueCount == 1 && LongVoiceLineQueueCount == 0)
        {
            return Queues[0];
        }
        
        var list = GetQueueListForInput(request);

        var bestQueue = list[0];
        var maxAmount = int.MaxValue;
        foreach (var item in list)
        {
            if (item.CountIncludingCurrentTask == 0)
            {
                return item;
            }

            if (item.CountIncludingCurrentTask < maxAmount)
            {
                bestQueue = item;
                maxAmount = item.Count;
            }
        }

        return bestQueue;
    }

    private static List<VoiceLineQueue> GetQueueListForInput(VoiceLineRequest request)
    {
        if (LongVoiceLineQueueCount == 0) return Queues;
        
        if (!string.IsNullOrEmpty(request.Input.Message) &&
            request.Input.Message.Length > LongVoiceLineMinCharacterCutOff)
        {
            return LongVoiceLineQueues;
        }

        return Queues;
    }
}