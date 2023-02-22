namespace Frends.JSON.QuerySingle.Definitions;

/// <summary>
/// Options parameters.
/// </summary>
public class Options
{
    /// <summary>
    /// A flag to indicate whether an error should be thrown if no tokens are found when evaluating part of the expression.
    /// </summary>
    /// <example>true</example>
    public bool ErrorWhenNotMatched { get; set; }
}