using System.Runtime.CompilerServices;

namespace Game.Utilities;

public static class TaskUtils
{
    public static ContinueAwaitable ContinueAt(TaskCreationOptions options) => new(options);

    public static ContinueAwaitable SwitchToLongRunning() => new(TaskCreationOptions.LongRunning);

    public readonly struct ContinueAwaitable(TaskCreationOptions options)
    {
        public ContinueAwaiter GetAwaiter() => new(options);
    }

    public readonly struct ContinueAwaiter(TaskCreationOptions options) : INotifyCompletion
    {
        public bool IsCompleted => false;

        public void OnCompleted(Action continuation)
        {
            Task.Factory.StartNew(continuation, options);
        }

        public void GetResult() { }
    }
}
