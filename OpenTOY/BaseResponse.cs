namespace OpenTOY;

public class BaseResponse
{
    public int ErrorCode { get; set; }
    public string ErrorText { get; set; } = string.Empty;
    public string ErrorDetail { get; set; } = string.Empty;
}