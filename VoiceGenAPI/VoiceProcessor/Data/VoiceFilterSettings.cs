using System.Text.Json.Serialization;

namespace VoiceProcessor.Data;

public class VoiceFilterSettings
{
    [JsonPropertyName("pitch_shift")]
    public float PitchShift { get; set; }
    [JsonPropertyName("fl_studio_path")]
    public string FlStudioPath { get; set; }
    [JsonPropertyName("fl_project_name_without_extension")]
    public string FlProjectNameWithoutExtension { get; set; }
    [JsonPropertyName("fl_project_containing_folder")]
    public string FlProjectContainingFolder { get; set; }
    [JsonPropertyName("padding")]
    public double Padding { get; set; }
}