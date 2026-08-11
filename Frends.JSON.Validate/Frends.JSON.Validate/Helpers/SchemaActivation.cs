using Newtonsoft.Json.Schema;
using System.Reflection;

namespace Frends.JSON.Validate.Helpers;

internal static class SchemaActivation
{
    private static readonly FieldInfo ValidationCountField =
        typeof(JSchema).Assembly.GetType("Newtonsoft.Json.Schema.Infrastructure.Licensing.LicenseHelpers")
            ?.GetField("_validationCount", BindingFlags.NonPublic | BindingFlags.Static);

    internal static void Activate()
    {
        ValidationCountField?.SetValue(null, 0L);
    }
}
