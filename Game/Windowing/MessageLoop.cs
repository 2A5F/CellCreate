using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Game.Native;

namespace Game.Windowing;

public static class MessageLoop
{
    internal static unsafe void InitMessageLoop()
    {
        ref var vtb = ref *App.s_native_app->MsgVtb();
        vtb.on_message = &OnMessage;
    }

    internal static unsafe void Loop()
    {
        (MessageThread = Thread.CurrentThread).Name = "Message Loop";
        App.s_native_app->MsgLoop().TryThrow();
    }

    private static Thread MessageThread { get; set; } = null!;

    private static ConcurrentQueue<Action> s_message_thread_requests = new();
    internal static SwitchToMessageThreadAwaitable SwitchToMessageThread() => new();

    internal struct SwitchToMessageThreadAwaitable
    {
        public SwitchToMessageThreadAwaiter GetAwaiter() => new();
    }

    internal struct SwitchToMessageThreadAwaiter : INotifyCompletion
    {
        public bool IsCompleted => MessageThread == Thread.CurrentThread;
        public unsafe void OnCompleted(Action continuation)
        {
            s_message_thread_requests.Enqueue(continuation);
            App.s_native_app->SendMsg(FMessage.SwitchThread, null);
        }
        public void GetResult() { }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static unsafe void OnMessage(FMessage type, void* data)
    {
        switch (type)
        {
            case FMessage.SwitchThread:
            {
                if (s_message_thread_requests.TryDequeue(out var cb))
                {
                    cb();
                }
                return;
            }
            case FMessage.WindowClose:
            {
                WindowManager.OnWindowClosed(new((uint)data));
                return;
            }
            case FMessage.WindowResize:
            {
                WindowManager.OnWindowResized(new((uint)data));
                return;
            }
        }
    }
}
