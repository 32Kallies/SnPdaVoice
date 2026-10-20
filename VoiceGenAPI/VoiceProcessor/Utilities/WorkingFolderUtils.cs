using System.Reflection;

namespace VoiceProcessor.Utilities;

public static class WorkingFolderUtils
{
    private static readonly Assembly MyAssembly = Assembly.GetExecutingAssembly();

    private const string RootFolderName = "voice-processing-container";
    private const string TempFolderName = "temp";
    private const string FilterFolderName = "filter";
    private const string VoicesFolder = "voices";
    private const string TelemetryFileName = "telemetry.db";

    private static string? _workingFolderPath;

    private static string GetWorkingFolderRoot()
    {
        if (_workingFolderPath == null)
        {
            var holdingPath = Path.Combine(Path.GetDirectoryName(MyAssembly.Location));
            if (new DirectoryInfo(holdingPath).Name == "net9.0")
            {
                holdingPath = Path.GetDirectoryName(Path.GetDirectoryName(
                    Path.GetDirectoryName(Path.GetDirectoryName(holdingPath))));
            }

            _workingFolderPath = Path.Combine(holdingPath, RootFolderName);
        }

        if (!Directory.Exists(_workingFolderPath))
        {
            Directory.CreateDirectory(_workingFolderPath);
        }

        return _workingFolderPath;
    }

    internal static string GetTempFolder()
    {
        var path = Path.Combine(GetWorkingFolderRoot(), TempFolderName);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        return path;
    }
    
    internal static string GetFilterFolder()
    {
        var path = Path.Combine(GetWorkingFolderRoot(), FilterFolderName);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        return path;
    }

    internal static string GetVoicesFolder()
    {
        var path = Path.Combine(GetWorkingFolderRoot(), VoicesFolder);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        return path;
    }

    public static string GetTelemetryFilePath()
    {
        return Path.Combine(GetWorkingFolderRoot(), TelemetryFileName);
    }
}