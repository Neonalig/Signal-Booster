using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;

namespace SignalBooster.Models;

[SuppressMessage("ReSharper", "InconsistentNaming")]
internal static class NativeMethods {
    public delegate int HookProc( int nCode, IntPtr wParam, IntPtr lParam );

    // [LibraryImport("user32.dll", SetLastError = true)]
    // internal static partial IntPtr SetWindowsHookEx( int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId );

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern IntPtr SetWindowsHookEx( int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId );

    // [LibraryImport("user32.dll", SetLastError = true)]
    // [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("user32.dll", SetLastError = true)]
    internal static extern bool UnhookWindowsHookEx( IntPtr hhk );

    // [LibraryImport("user32.dll")]
    [DllImport("user32.dll")]
    internal static extern int CallNextHookEx( IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam );

    // [LibraryImport("user32.dll", StringMarshalling = StringMarshalling.Utf16)]
    // internal static partial IntPtr GetModuleHandle( string lpModuleName );
    
    internal static IntPtr GetModuleHandle( Module Module ) => Marshal.GetHINSTANCE(Module);
    internal static IntPtr GetModuleHandle() => Marshal.GetHINSTANCE(typeof(NativeMethods).Module);

    // [LibraryImport("user32.dll")]
    [DllImport("user32.dll")]
    internal static extern void keybd_event( byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo );

    public const int WH_KEYBOARD_LL = 13;
    public const int WM_KEYDOWN     = 0x0100;
    public const int WM_KEYUP       = 0x0101;
}
