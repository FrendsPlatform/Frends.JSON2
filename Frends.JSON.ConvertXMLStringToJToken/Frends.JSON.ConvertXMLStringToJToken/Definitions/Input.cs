namespace Frends.JSON.ConvertXMLStringToJToken.Definitions;

/// <summary>
/// Input parameter.
/// </summary>
public class Input
{
    /// <summary>
    /// XML string to convert.
    /// </summary>
    /// <example>&lt;?xml version='1.0' standalone='no'?&gt;&lt;root&gt;&lt;foos id = '1' &gt;&lt;foo&gt;bar&lt;/name&gt;&lt;/foos&gt;&lt;/root&gt;</example>
    public string XML { get; set; }
}