﻿using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Game.Native;

namespace Game;

internal static class Entry
{
    [UnmanagedCallersOnly]
    private static unsafe void Init(InitParams* init_params, InitResult* init_result)
    {
        App.s_appVars = init_params->p_vas;
        App.s_native_app = init_params->p_native_app;
        init_result->fn_vtb->main = &Main;
    }
    
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void Main()
    {
        App.Main();
    }
}
