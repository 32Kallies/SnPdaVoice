using System.Text.RegularExpressions;

namespace VoiceProcessor;

public class PronunciationHelper
{
    private static readonly Dictionary<string, string> SpecialPronunciations = new(StringComparer.OrdinalIgnoreCase)
    {
        { "pda", "peedee A" },
        { "subnautica", "sub not ic uh" }
    };
    
    public string Pronounce(string message)
    {
        var words = GetWords(message);
        int currentOffset = 0;
        string final = message;
        foreach (var word in words)
        {
            if (SpecialPronunciations.TryGetValue(word.Text, out var replacement))
            {
                int replaceIndex = word.Index + currentOffset;
                final = final.Remove(replaceIndex, word.Length);
                final = final.Insert(replaceIndex, replacement);
                currentOffset += replacement.Length - word.Length;
            }
        }

        return final;
    }

    private static Word[] GetWords(string input)
    {
        return Regex.Matches(input, @"\b[\w]+\b")
            .Select(m => new Word(m.Index, m.Value))
            .ToArray();
    }

    private struct Word(int index, string text)
    {
        public int Index = index;
        public string Text = text;
        public int Length = text.Length;
    }
}