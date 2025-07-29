using System.Collections;

namespace Runeforge.Core.Extensions.Env;

public static class EnvExtensions
{
    public static string ExpandEnvironmentVariables(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        foreach (DictionaryEntry env in Environment.GetEnvironmentVariables())
        {
            var key = $"${env.Key}";
            var value = env.Value?.ToString() ?? "";
            input = input.Replace(key, value);
        }

        return input;
    }
}
