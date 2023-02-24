﻿namespace Frends.JSON.Handlebars.Definitions;

/// <summary>
/// Task's result.
/// </summary>
public class Result
{
    /// <summary>
    /// Operation complete without errors.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; private set; }

    /// <summary>
    /// Result data.
    /// </summary>
    /// <example>&lt;div&gt;&lt;span&gt;Mr.&lt;/span&gt; &lt;strong&gt;Andersson&lt;/strong&gt;&lt;/div&gt;</example>
    public string Data { get; private set; }

    internal Result(bool success, string data)
    {
        Success = success;
        Data = data;
    }
}