using System.Text.RegularExpressions;

namespace VoiceProcessor;

public class Filter
{
    private string[] _blacklistedWords = [];
    
    public void LoadFromFolder(string folder, string fileName = "filter.txt")
    {
        var path = Path.Combine(folder, fileName);
        if (!File.Exists(path))
        {
            Console.WriteLine($"Could not find a file by name '{fileName}' in '{folder}'");
            return;
        }
        Console.WriteLine("Loading filter...");
        _blacklistedWords = File.ReadAllText(path)
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(entry => entry.Trim())
            .ToArray();
    }

    public string RemoveBlacklistedWords(string input)
    {
        string result = input;
        foreach (var blacklisted in _blacklistedWords)
        {
            result = Regex.Replace(result, blacklisted, "redacted", RegexOptions.IgnoreCase);
        }

        return result;
    }
}