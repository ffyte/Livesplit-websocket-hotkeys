// See https://aka.ms/new-console-template for more information

using Websocket.Client;
using System.Runtime.InteropServices;

internal class Program
{
    public static ConsoleKeyInfo startorsplit = new();
    public static ConsoleKeyInfo reset = new();
    public static ConsoleKeyInfo unsplit = new();
    public static string message = "empty";
    public static bool newmessage = false;
    private static void Main()
    {
        //startup and setting saving
        
        
        string ip = "127.0.0.1:16835";
        

        Console.WriteLine("press Y to delete settings?");
        if (Console.ReadKey().KeyChar == 'Y')
        {
            File.Delete("settings.txt");
        }
        Console.WriteLine("");

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

            var outputfile = new StreamWriter("settings.txt");
            if (ip != null) outputfile.WriteLine(ip.ToString());

            outputfile.WriteLine((char)startorsplit.Key + " " + startorsplit.Modifiers.ToString());
            outputfile.WriteLine((char)reset.Key + " " + reset.Modifiers.ToString());
            outputfile.WriteLine((char)unsplit.Key + " " + unsplit.Modifiers.ToString());
            outputfile.Close();
        }

        //set Global hotkeys

        Console.WriteLine("registering keys");
        //WinInterop.RegisterHotKey(IntPtr.Zero, 0, (int)startorsplit.Modifiers +0x4000, (uint) startorsplit.Key);
        //WinInterop.RegisterHotKey(IntPtr.Zero, 1, (int)reset.Modifiers + 0x4000, (uint) reset.Key);
        //WinInterop.RegisterHotKey(IntPtr.Zero, 2, (int)unsplit.Modifiers + 0x4000, (uint) unsplit.Key);

        
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

        IntPtr hook = WinInterop.SetWindowsHookEx(13, WinInterop.HookCallback, IntPtr.Zero, 0);

        bool abortpressed =false;
        Console.CancelKeyPress += delegate(object? sender, ConsoleCancelEventArgs e) {
            // call methods to clean up
            Console.WriteLine("exiting...");
            abortpressed = true;
            //WinInterop.UnregisterHotKey(IntPtr.Zero, 0);
            //WinInterop.UnregisterHotKey(IntPtr.Zero, 1);
            //WinInterop.UnregisterHotKey(IntPtr.Zero, 2);
            WinInterop.UnhookWindowsHookEx(hook);
            System.Environment.Exit(0);
        };
        Console.WriteLine("ctrl+c to exit");

        
  

  
        //main loop
        
        while (!abortpressed)
        {
        
                if (Program.newmessage)
                {
                    Console.WriteLine(Program.message);
                    client.Send(Program.message);
                    Program.newmessage = false;
                }
            /* comment out
            
            if (WinInterop.GetMessageW(out WinInterop.Message msg, IntPtr.Zero, 0, 0, 1))
            {
                Console.WriteLine(msg.Msg);
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
                    Console.WriteLine(message);
                    client.Send(message);
                }
            }*/
        }
    }

}



static partial class WinInterop
{

    const int WH_KEYBOARD_LL = 13;
    const int WM_KEYDOWN = 0x0100;
    const int WM_KEYUP = 0x0101;

    public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
    public static event Action<Keys, bool> KeyAction;
    public static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_KEYUP))
        {
            int vkCode = Marshal.ReadInt32(lParam);
            ConsoleKeyInfo keyInfo = new ((char)0, (ConsoleKey)vkCode, false, false, false);
            Console.WriteLine(wParam);
            if (wParam == (IntPtr)WM_KEYDOWN)
            {
                Console.WriteLine("Key pressed: " + keyInfo.Key);
                if (keyInfo.Key == Program.startorsplit.Key)
                {
                    Program.message = "startorsplit";
                    Program.newmessage = true;
                }
                else if (keyInfo.Key == Program.reset.Key) {
                    Program.message = "reset";
                    Program.newmessage = true;
                }
                else if (keyInfo.Key == Program.unsplit.Key)
                {
                    Program.message = "unsplit";
                    Program.newmessage = true;
                }
                
            }
        }

        return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
       
    }

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

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

}
