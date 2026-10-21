using System.Text.RegularExpressions;
using VoiceProcessor.Utilities;

namespace VoiceProcessor;

public class TextPreprocessor
{
    private readonly PronunciationHelper _pronunciation;
    private readonly Filter _filter;
    
    public TextPreprocessor()
    {
        _filter = new Filter();
        _filter.LoadFromFolder(WorkingFolderUtils.GetFilterFolder());
        _pronunciation = new PronunciationHelper();
    }

    public string PreprocessText(string text, bool applyFilter = true, bool fixPronunciation = true, bool unescape = true, bool removeNewlines = false)
    {
        string result = text;

        if (unescape)
            result = Regex.Unescape(result);
        
        if (removeNewlines)
            result = result.Replace("\n", " ");

        if (applyFilter)
            result = _filter.RemoveBlacklistedWords(result);
        
        if (fixPronunciation)
            result = _pronunciation.Pronounce(result);
        
        return result;
    }
}