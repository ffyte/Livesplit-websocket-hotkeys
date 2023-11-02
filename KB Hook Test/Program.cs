using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
internal class Program
{
    const int WH_KEYBOARD_LL = 13;
    const int WM_KEYDOWN = 0x0100;
    const int WM_KEYUP = 0x0101;
    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
    private static event Action<Keys, bool> KeyAction;
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_KEYUP))
        {
            int vkCode = Marshal.ReadInt32(lParam);
            Keys key = (Keys)vkCode;

            // Invoke the KeyAction event to handle the key press/release
            KeyAction?.Invoke(key, wParam == (IntPtr)WM_KEYDOWN);
        }

        return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
    }

    static void Main()
    {
        IntPtr hook = SetWindowsHookEx(WH_KEYBOARD_LL, HookCallback, IntPtr.Zero, 0);

        if (hook == IntPtr.Zero)
        {
            Console.WriteLine("Failed to set up the keyboard hook.");
            return;
        }

        KeyAction += (key, isKeyDown) =>
        {
            if (key == Keys.A && isKeyDown)
            {
                
                Console.WriteLine("Hotkey pressed!");
                // Handle your hotkey action here
            }
        };

        Console.WriteLine("Press the 'A' key to trigger the hotkey. Press 'Q' to exit.");

        while (true)
        {
            if (Console.KeyAvailable)
            {
                if (Console.ReadKey(intercept: true).Key == ConsoleKey.Q)
                {
                    break;
                }
            }
        }

        UnhookWindowsHookEx(hook);
    }

}