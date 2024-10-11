namespace Game.Native
{
    public unsafe partial struct AppFnVtb
    {
        [NativeTypeName("FFI_Action *")]
        public delegate* unmanaged[Cdecl]<void> main;
    }

    public partial struct AppVars
    {
        [NativeTypeName("cc::b8")]
        public B8 debug;
    }

    public unsafe partial struct InitParams
    {
        [NativeTypeName("cc::AppVars *")]
        public AppVars* p_vas;
    }

    public unsafe partial struct InitResult
    {
        [NativeTypeName("cc::AppFnVtb *")]
        public AppFnVtb* fn_vtb;
    }
}
