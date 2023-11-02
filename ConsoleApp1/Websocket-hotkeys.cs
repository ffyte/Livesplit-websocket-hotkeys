// See https://aka.ms/new-console-template for more information

using Websocket.Client;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static WinInterop;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

internal class Program
{
    public static ConsoleKeyInfo startorsplit = new();
    public static ConsoleKeyInfo reset = new();
    public static ConsoleKeyInfo unsplit = new();
    public static ConsoleKeyInfo skipsplit = new();
    public static ConsoleKeyInfo pause = new();
    public static string message = "empty";
    public static bool newmessage = false;
    
    private static void Main()
    {
        //startup and setting saving

        bool paused = false;
        string ip = "127.0.0.1:16835";

        bool fallback = false;
        if (File.Exists("settings.txt"))
        {
            Console.WriteLine("press Y to delete settings?");
            if (Console.ReadKey().KeyChar == 'Y')
            {
                File.Delete("settings.txt");
            }
            Console.WriteLine("");
        }
        

        if (File.Exists("settings.txt"))
        {

            bool altmod = false;
            bool shiftmod = false;
            bool controlmod = false;
            var inputfile = new StreamReader("settings.txt");
            char key;
            if (inputfile != null) { }
            ip = inputfile.ReadLine();
            Console.WriteLine("ip set: " + ip);
            string inputstring = inputfile.ReadLine();
            //Console.WriteLine(inputstring);
            if (inputstring != null)
            {
                if (inputstring.Contains("alt")) { altmod = true; }
                if (inputstring.Contains("shift")) { shiftmod = true; }
                if (inputstring.Contains("ctrl")) { controlmod = true; }

                key = char.Parse(inputstring.Split(' ').FirstOrDefault());
                startorsplit = new ConsoleKeyInfo(key, (ConsoleKey)key, shiftmod, altmod, controlmod);
                Console.WriteLine("startsplit: " + startorsplit.Key + " " + startorsplit.GetHashCode());
            }
            altmod = shiftmod = controlmod = false;
            inputstring = inputfile.ReadLine();
            //Console.WriteLine(inputstring);
            if (inputstring != null)
            {
                if (inputstring.Contains("alt")) { altmod = true; }
                if (inputstring.Contains("shift")) { shiftmod = true; }
                if (inputstring.Contains("ctrl")) { controlmod = true; }

                key = char.Parse(inputstring.Split(' ').FirstOrDefault());
                reset = new ConsoleKeyInfo(key, (ConsoleKey)key, shiftmod, altmod, controlmod);
                Console.WriteLine("reset: " + reset.Key + " " + reset.GetHashCode());
            }
            altmod = shiftmod = controlmod = false;
            inputstring = inputfile.ReadLine();
            //Console.WriteLine(inputstring);
            if (inputstring != null)
            {
                if (inputstring.Contains("alt")) { altmod = true; }
                if (inputstring.Contains("shift")) { shiftmod = true; }
                if (inputstring.Contains("ctrl")) { controlmod = true; }
                key = char.Parse(inputstring.Split(' ').FirstOrDefault());
                unsplit = new ConsoleKeyInfo(key, (ConsoleKey)key, shiftmod, altmod, controlmod);
                Console.WriteLine("unsplit: " + unsplit.Key.ToString() + " " + unsplit.KeyChar.GetHashCode());
            }
            altmod = shiftmod = controlmod = false;
            inputstring = inputfile.ReadLine();
            //Console.WriteLine(inputstring);
            if (inputstring != null)
            {
                if (inputstring.Contains("alt")) { altmod = true; }
                if (inputstring.Contains("shift")) { shiftmod = true; }
                if (inputstring.Contains("ctrl")) { controlmod = true; }
                key = char.Parse(inputstring.Split(' ').FirstOrDefault());
                pause = new ConsoleKeyInfo(key, (ConsoleKey)key, shiftmod, altmod, controlmod);
                Console.WriteLine("pause: " + pause.Key.ToString() + " " + pause.KeyChar.GetHashCode());
            }
            altmod = shiftmod = controlmod = false;
            inputstring = inputfile.ReadLine();
            //Console.WriteLine(inputstring);
            if (inputstring != null)
            {
                if (inputstring.Contains("alt")) { altmod = true; }
                if (inputstring.Contains("shift")) { shiftmod = true; }
                if (inputstring.Contains("ctrl")) { controlmod = true; }
                key = char.Parse(inputstring.Split(' ').FirstOrDefault());
                skipsplit = new ConsoleKeyInfo(key, (ConsoleKey)key, shiftmod, altmod, controlmod);
                Console.WriteLine("skipsplit: " + skipsplit.Key.ToString() + " " + skipsplit.KeyChar.GetHashCode());
            }
            inputfile.Close();



        }

        else
        {
            Console.WriteLine("Enter IP and port (example: 127.0.0.1:16835)");
            ip = Console.ReadLine();
            Console.WriteLine("start and split key");
            startorsplit = Console.ReadKey();
            Console.WriteLine();
            Console.WriteLine("reset key");
            reset = Console.ReadKey();
            Console.WriteLine();
            Console.WriteLine("undo split key");
            unsplit = Console.ReadKey();
            Console.WriteLine();
            Console.WriteLine("pause key");
            pause = Console.ReadKey();
            Console.WriteLine();
            Console.WriteLine("skip split key");
            skipsplit = Console.ReadKey();


            var outputfile = new StreamWriter("settings.txt");
            if (ip != null) outputfile.WriteLine(ip.ToString());

            outputfile.WriteLine((char)startorsplit.Key + " " + startorsplit.Modifiers.ToString());
            outputfile.WriteLine((char)reset.Key + " " + reset.Modifiers.ToString());
            outputfile.WriteLine((char)unsplit.Key + " " + unsplit.Modifiers.ToString());
            outputfile.WriteLine((char)pause.Key + " " + pause.Modifiers.ToString());
            outputfile.WriteLine((char)skipsplit.Key + " " + skipsplit.Modifiers.ToString());
            outputfile.Close();
        }

        //set Global hotkeys

        Console.WriteLine("registering keys");
        if (fallback)
        {
            Console.WriteLine("Fallback blocking keys active");
            WinInterop.RegisterHotKey(IntPtr.Zero, 0, (int)startorsplit.Modifiers +0x4000, (uint) startorsplit.Key);
            WinInterop.RegisterHotKey(IntPtr.Zero, 1, (int)reset.Modifiers + 0x4000, (uint) reset.Key);
            WinInterop.RegisterHotKey(IntPtr.Zero, 2, (int)unsplit.Modifiers + 0x4000, (uint) unsplit.Key);
            WinInterop.RegisterHotKey(IntPtr.Zero, 3, (int)pause.Modifiers + 0x4000, (uint) pause.Key);
            WinInterop.RegisterHotKey(IntPtr.Zero, 4, (int)skipsplit.Modifiers + 0x4000, (uint)skipsplit.Key);
        }
        
        //Communication to server
        var url = new Uri("ws://" + ip + "/livesplit");
        using var client = new WebsocketClient(url);

        client.Start();

        if (!client.IsRunning)
        {
            Console.WriteLine("Couldn't connect to server?");
            //System.Environment.Exit(1); 
        }
        
        Console.WriteLine("Connected to: " + url);


        bool abortpressed =false;
        IntPtr hook = IntPtr.Zero;
        Console.CancelKeyPress += delegate(object? sender, ConsoleCancelEventArgs e) {
            // call methods to clean up
            Console.WriteLine("exiting...");
            abortpressed = true;
            if (fallback)
            {
                WinInterop.UnregisterHotKey(IntPtr.Zero, 0);
                WinInterop.UnregisterHotKey(IntPtr.Zero, 1);
                WinInterop.UnregisterHotKey(IntPtr.Zero, 2);
                WinInterop.UnregisterHotKey(IntPtr.Zero, 3);
                WinInterop.UnregisterHotKey(IntPtr.Zero, 4);
            }
            WinInterop.UnhookWindowsHookEx(hook);
            System.Environment.Exit(0);
        };




        //main loop

        hook = WinInterop.SetWindowsHookEx(WH_KEYBOARD_LL, WinInterop.HookCallback, GetModuleHandleA(Process.GetCurrentProcess().MainModule.ModuleName), 0);
        if (hook == IntPtr.Zero)
        {
            Console.WriteLine("Failed to hook");
            return;
        }
        Program.message = "startorsplit";

       
        
        //WinInterop.Message msg = new();
        Console.WriteLine("ctrl+c to exit");
        while ( !abortpressed)
        {
            WinInterop.PeekMessageA(out WinInterop.Message msg, IntPtr.Zero, 0, 0, 1);
            //Console.WriteLine(msg);u
            /*if (hook == IntPtr.Zero)
            {
                Console.WriteLine("Failed to hook");
                hook = WinInterop.SetWindowsHookEx(WH_KEYBOARD_LL, WinInterop.HookCallback, 0, 0);
            }*/
            if (Program.newmessage)
            {

                if (paused) { Program.message = "resume";paused=false;}
                Console.WriteLine(Program.message);
                client.Send(Program.message);
                if(Program.message == "pause") { paused = true; }
                Program.newmessage = false;
                }
            //Console.WriteLine(Program.newmessage);
            if (fallback)
            {


               
                    if (Program.newmessage)
                    {
                        Console.WriteLine(Program.message);
                        client.Send(Program.message);
                        Program.newmessage = false;
                    }
                    //Console.WriteLine(msg.Msg);
                    if (msg.Msg == WinInterop.WM_HOTKEY)
                    {
                        var param = msg.WParam.ToInt32();
                        if (param == 0)
                        {
                            message = "startorsplit";
                        }
                        else if (param == 1)
                        {
                            message = "reset";
                        }
                        else if (param == 2)
                        {
                            message = "unsplit";
                        }
                        else if (param == 3)
                        {
                            message = "pause";
                        }
                        else if (param == 4)
                        {
                            message = "skipsplit";
                        }
                        Console.WriteLine(message);
                        client.Send(message);
                    }
                
            }
        }
        UnhookWindowsHookEx(hook);
        
    }

    
}



static partial class WinInterop
{

    public const int WH_KEYBOARD_LL = 13;
    const int WM_KEYDOWN = 0x0100;
    const int WM_KEYUP = 0x0101;

    public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
    public static event Action<Keys, bool> KeyAction;


    public const int WM_HOTKEY = 0x312;
    
    [LibraryImport("user32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool RegisterHotKey(
        IntPtr hWnd,
        int id,
        int fsModifiers,
        uint vk
        );
    [LibraryImport("user32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetMessageW(
        out Message msg,
        IntPtr hWnd,
        uint wMsgFilterMin,
        uint wMsgFilterMax,
        uint remove
        );
    [LibraryImport("user32.dll")]
    [return: MarshalAs (UnmanagedType.Bool)]
    public static partial bool UnregisterHotKey(IntPtr hWnd, int id);
    public struct Message
    {
        public IntPtr HWnd;
        public uint Msg;
        public IntPtr WParam;
        public IntPtr LParam;
        public uint Time;
    }

    public static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_KEYUP))
        {
           
            int vkCode = Marshal.ReadInt32(lParam);
            ConsoleKeyInfo keyInfo = new((char)0, (ConsoleKey)vkCode, false, false, false);
            if (wParam == (IntPtr)WM_KEYUP)
            {
                //Console.WriteLine("Key pressed: " + keyInfo.Key);
                
                if (keyInfo.Key == Program.startorsplit.Key)
                {
                    
                    Program.message = "startorsplit";
                    Program.newmessage = true;
                    //Console.WriteLine(Program.startorsplit.Key + Program.message);
                }
                else if (keyInfo.Key == Program.reset.Key)
                {
                    
                    Program.message = "reset";
                    Program.newmessage = true;
                    //Console.WriteLine(Program.reset.Key+Program.message);
                }
                else if (keyInfo.Key == Program.unsplit.Key)
                {
                    
                    Program.message = "unsplit";
                    Program.newmessage = true;
                    //Console.WriteLine(Program.unsplit.Key + Program.message);
                }
                else if (keyInfo.Key == Program.pause.Key) 
                {
                    Program.message = "pause";
                    Program.newmessage = true;
                }
                else if (keyInfo.Key == Program.skipsplit.Key)
                {
                    Program.message = "skipsplit";
                    Program.newmessage = true;
                }
            }
        }

        return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);

    }

    [LibraryImport("user32.dll", EntryPoint = "SetWindowsHookExW", SetLastError = true)]
    public static partial IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [LibraryImport("user32.dll", EntryPoint = "UnhookWindowsHookEx", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool UnhookWindowsHookEx(IntPtr hhk);

    [LibraryImport("user32.dll", EntryPoint = "CallNextHookEx", SetLastError = true)]
    public static partial IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [LibraryImport("kernel32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    public static partial IntPtr GetModuleHandleA(string lpModuleName);
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool PeekMessageA(out Message msg, IntPtr hWnd, uint filterMin, uint filterMax, uint remove);
}
