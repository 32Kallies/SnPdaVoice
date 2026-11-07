namespace VoiceProcessor.Data;

public class FlStudioResult
{
    public bool Success { get; private set; }

    public void SetSuccess(bool succeeded)
    {
        Success = succeeded;
    }
}