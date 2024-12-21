using FastEndpoints;

namespace OpenTOY;

public class BaseRequest
{
    [FromHeader]
    public string AcceptCountry { get; set; } = string.Empty;
    [FromHeader]
    public string AcceptLanguage { get; set; } = string.Empty;
    [FromHeader]
    public string Uuid { get; set; } = string.Empty;
    [FromHeader("npsn")]
    public long Id { get; set; }
    [FromHeader]
    public required NpParams NpParams { get; set; }
}

public class NpParams
{
    public string SdkVer { get; set; } = string.Empty;
    public string Os { get; set; } = string.Empty;
    public string SvcId { get; set; } = string.Empty;
    public string NpToken { get; set; } = string.Empty;
}