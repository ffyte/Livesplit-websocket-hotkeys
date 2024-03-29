﻿
using Websocket.Client;
using System.Runtime.InteropServices;
using System.Diagnostics;

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
        //for resume state
        bool paused = false;
        string ip = "127.0.0.1:16835";
        IntPtr hook = IntPtr.Zero;
        //WinInterop.LowLevelKeyboardProcMethod hookProcDelegate;

        //for blocking hotkeys
        bool fallback = false;
        if (File.Exists("fallback"))
        {
            fallback = true;
            Console.WriteLine("Using blocking hotkeys");
        }

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
            Console.WriteLine("");
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
        } else
        {
            WinInterop.LowLevelKeyboardProcMethod = WinInterop.HookCallback;
            IntPtr hInstance = WinInterop.LoadLibrary("User32");
            hook = WinInterop.SetWindowsHookEx(13, WinInterop.LowLevelKeyboardProcMethod, hInstance, 0);
            if (hook == IntPtr.Zero)
            {
                Console.WriteLine("Failed to hook");
                return;
            }
        }
        
        //Communication to server
        var url = new Uri("ws://" + ip + "/livesplit");
        using var client = new WebsocketClient(url);

        client.Start();

        
        Console.WriteLine("Connected to: " + url);


        bool abortpressed =false;
        
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

        
        message = "startorsplit";

        Stopwatch stopwatch = new ();
        WinInterop.Message msg = new();


        Console.WriteLine("ctrl+c to exit");
        while ( !abortpressed)
        {
            if (!fallback) { WinInterop.PeekMessage(out msg, IntPtr.Zero, 0, 0, 1); }
            
            //prevent extra keypresses for non-blocking, don't know how to do it for blocking
            if ((newmessage && message == "startorsplit") && stopwatch.ElapsedMilliseconds < 300 && stopwatch.IsRunning)
            {
                newmessage=false;
                
            }
            
            

            if (stopwatch.ElapsedMilliseconds >= 300)
            {
                stopwatch.Reset();
            }

            

            if (newmessage && !stopwatch.IsRunning)
            {
                
                if (paused) { message = "resume";paused=false;}
                Console.WriteLine(message);
                client.Send(message);
                if(message == "pause") { paused = true; }
                newmessage = false;
                
                if (message== "startorsplit") stopwatch.Start();
                }
            
            if (fallback && !stopwatch.IsRunning)
            {


                WinInterop.GetMessageW(out msg, IntPtr.Zero, 0, 0, 1);

                

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
                        
                        if (paused) message = "resume"; paused = false;
                        if (message == "pause") paused = true;    
                            
                        }
                        else if (param == 4)
                        {
                            message = "skipsplit";
                        }
                    
                        Console.WriteLine(message);
                        client.Send(message);
                        if (message == "startorsplit") stopwatch.Start();
                    
                    }
                
            }
        }
        WinInterop.UnhookWindowsHookEx(hook);
        
    }

    
}



static partial class WinInterop
{

    public const int WH_KEYBOARD_LL = 13;
    const int WM_KEYDOWN = 0x0100;
    const int WM_KEYUP = 0x0101;

    public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
    public static LowLevelKeyboardProc LowLevelKeyboardProcMethod;


    public const int WM_HOTKEY = 0x312;
    [DllImport("user32.dll")]
    public static extern bool RegisterHotKey(
        IntPtr hWnd,
        int id,
        int fsModifiers,
        uint vk
        );
    [DllImport("user32.dll")]
    public static extern bool GetMessageW(
        out Message msg,
        IntPtr hWnd,
        uint wMsgFilterMin,
        uint wMsgFilterMax,
        uint remove
        );
    [DllImport("user32.dll")]
    public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
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

    [DllImport("user32.dll")]
    public static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll")]
    public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    public static extern bool PeekMessage(out Message msg, IntPtr hWnd, uint filterMin, uint filterMax, uint remove);
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr LoadLibrary(string lpFileName);
}
