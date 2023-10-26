using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Input;

namespace SignalBooster.Models;

/// <summary> Provides methods for simulating keyboard input via Windows API calls. Also allows subscribing to keyboard events, such as for global hotkeys (whether with modifiers or not). </summary>
public static class Keyboard {
    static Keyboard() {
        _HookDelegate                       =  HookCallback;
        AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
    }

    #region Hooking

    static IntPtr _HookID = IntPtr.Zero;

    static readonly NativeMethods.HookProc _HookDelegate;

    /// <summary> Event handler for keyboard events. </summary>
    /// <param name="Key"> The key that was pressed or released. </param>
    public delegate void KeyEventHandler( Key Key );

    // ReSharper disable InconsistentNaming
    static event KeyEventHandler? _KeyDown;
    static event KeyEventHandler? _KeyUp;
    // ReSharper restore InconsistentNaming

    /// <summary> Raised when a key is pressed. </summary>
    public static event KeyEventHandler KeyDown {
        add {
            if (!IsHooked) { Hook(); }

            _KeyDown += value;
        }
        remove {
            _KeyDown -= value;
            if (_KeyDown is null && _KeyUp is null) { Unhook(); }
        }
    }

    /// <summary> Raised when a key is released. </summary>
    public static event KeyEventHandler KeyUp {
        add {
            if (!IsHooked) { Hook(); }

            _KeyUp += value;
        }
        remove {
            _KeyUp -= value;
            if (_KeyUp is null && _KeyDown is null) { Unhook(); }
        }
    }

    static bool IsHooked => _HookID != IntPtr.Zero;

    static void Hook() {
        if (IsHooked) {
            Console.WriteLine("Already hooked.");
            return;
        }

        Console.WriteLine("Hooking keyboard.");
        _HookID = SetHook(_HookDelegate);
    }

    static IntPtr SetHook( NativeMethods.HookProc Proc ) =>
        // using Process        CurProcess = Process.GetCurrentProcess();
        // using ProcessModule? CurModule  = CurProcess.MainModule;
        // if (CurModule is null) { throw new NullReferenceException("Current process has no main module."); }
        // return NativeMethods.SetWindowsHookEx(NativeMethods.WH_KEYBOARD_LL, Proc, NativeMethods.GetModuleHandle(CurModule.ModuleName), 0);
        NativeMethods.SetWindowsHookEx(NativeMethods.WH_KEYBOARD_LL, Proc, NativeMethods.GetModuleHandle(), 0);

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    static int HookCallback( int nCode, IntPtr wParam, IntPtr lParam ) {
        if (nCode >= 0) {
            int VkCode = Marshal.ReadInt32(lParam);
            switch (wParam) {
                case NativeMethods.WM_KEYDOWN:
                    _KeyDown?.Invoke(KeyInterop.KeyFromVirtualKey(VkCode));
                    break;
                case NativeMethods.WM_KEYUP:
                    _KeyUp?.Invoke(KeyInterop.KeyFromVirtualKey(VkCode));
                    break;
                default:
                    Trace.TraceInformation($"Unknown key event: {wParam}");
                    break;
            }
        }

        return NativeMethods.CallNextHookEx(_HookID, nCode, wParam, lParam);
    }

    static void Unhook() {
        if (!IsHooked) {
            Console.WriteLine("Already unhooked.");
            return;
        }

        Console.WriteLine("Unhooking keyboard.");
        NativeMethods.UnhookWindowsHookEx(_HookID);
        _HookID = IntPtr.Zero;
    }

    #endregion

    #region Methods

    /// <summary> Simulates a key press. </summary>
    /// <param name="Key"> The key to press. </param>
    public static void Press( Key Key ) => NativeMethods.keybd_event((byte)KeyInterop.VirtualKeyFromKey(Key), 0, 0, UIntPtr.Zero);

    /// <summary> Simulates a key release. </summary>
    /// <param name="Key"> The key to release. </param>
    public static void Release( Key Key ) => NativeMethods.keybd_event((byte)KeyInterop.VirtualKeyFromKey(Key), 0, 2, UIntPtr.Zero);

    #endregion

    // sealed class Destructor {
    //     ~Destructor() => Unhook();
    // }
    // static readonly Destructor _Destructor = new();
    static void OnProcessExit( object? Sender, EventArgs E ) => Unhook();

}