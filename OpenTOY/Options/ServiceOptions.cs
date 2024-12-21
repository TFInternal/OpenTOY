using System.ComponentModel.DataAnnotations;

namespace OpenTOY.Options;

[OptionsSection("ServiceSettings")]
public class ServiceOptions
{
    [Required]
    public Dictionary<string, ToyServiceInfo> Services { get; set; } = new();
}

public class ToyServiceInfo
{
    [Required]
    public string Title { get; set; } = string.Empty;
    [Required]
    public List<int> LoginMethods { get; set; } = [];
}