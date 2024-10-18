using System.Reflection;

namespace Game.Utilities;

public static class Utils
{
    public static string GetAsmVer(Assembly assembly)
    {
        var assemblyVersionAttribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

        if (assemblyVersionAttribute is null)
        {
            return assembly.GetName().Version?.ToString() ?? "";
        }
        else
        {
            return assemblyVersionAttribute.InformationalVersion;
        }
    }

    public static V GetOrAdd<K, V>(this Dictionary<K, V> dict, K key, Func<K, V> factory) where K : notnull
    {
        if (dict.TryGetValue(key, out var value)) return value;
        return dict[key] = factory(key);
    }
}
