using System.Reflection;

namespace Game.Utilities;

public class Utils
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
}
