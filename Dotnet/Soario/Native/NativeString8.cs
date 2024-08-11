﻿using System.Runtime.CompilerServices;
using System.Text;

namespace Soario.Native;

public sealed unsafe class NativeString8 : IDisposable
{
    #region Fields

    internal FString8* m_inner;

    #endregion

    #region Ctor

    public NativeString8(ReadOnlySpan<byte> bytes)
    {
        fixed (byte* ptr = bytes)
        {
            m_inner = FString8.Create(new() { ptr = ptr, len = (nuint)bytes.Length });
        }
    }

    public NativeString8(string str)
    {
        var bytes = Encoding.UTF8.GetBytes(str);
        fixed (byte* ptr = bytes)
        {
            m_inner = FString8.Create(new() { ptr = ptr, len = (nuint)bytes.Length });
        }
    }

    #endregion

    #region Props

    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_inner == null ? 0 : (int)m_inner->m_len;
    }

    public ReadOnlySpan<byte> AsSpan
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_inner == null ? default : new(m_inner->m_ptr, (int)m_inner->m_len);
    }

    #endregion

    #region ToString

    public override string ToString() => Encoding.UTF8.GetString(AsSpan);

    #endregion

    #region Dispose

    private void ReleaseUnmanagedResources()
    {
        if (m_inner == null) return;
        m_inner->Release();
        m_inner = null;
    }
    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }
    ~NativeString8() => ReleaseUnmanagedResources();

    #endregion
}