using Game.Native;

namespace Game;

public static unsafe class App
{
    internal static AppVars* s_appVars;

    public static ref AppVars Vars => ref *s_appVars;

    internal static void Main()
    {
        Console.WriteLine("From c#");
    }
}
