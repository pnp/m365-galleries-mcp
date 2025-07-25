namespace SampleGalleriesMCPServer;

/// <summary>
/// Configuration settings for the Samples API
/// </summary>
public class SamplesApiConfiguration
{
    public const string SectionName = "SamplesApi";
    
    /// <summary>
    /// The base URL of the Samples API
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;
}
