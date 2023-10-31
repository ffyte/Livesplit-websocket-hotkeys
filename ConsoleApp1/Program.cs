// See https://aka.ms/new-console-template for more information

using Websocket.Client;
using System.Runtime.InteropServices;


internal class Program
{
    private static void Main()
    {
        //startup and setting saving
        ConsoleKeyInfo startorsplit = new();
        ConsoleKeyInfo reset = new();
        ConsoleKeyInfo unsplit = new();
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
        WinInterop.RegisterHotKey(IntPtr.Zero, 0, (int)startorsplit.Modifiers +0x4000, (uint) startorsplit.Key);
        WinInterop.RegisterHotKey(IntPtr.Zero, 1, (int)reset.Modifiers + 0x4000, (uint) reset.Key);
        WinInterop.RegisterHotKey(IntPtr.Zero, 2, (int)unsplit.Modifiers + 0x4000, (uint) unsplit.Key);


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
        

        bool abortpressed=false;
        Console.CancelKeyPress += delegate(object? sender, ConsoleCancelEventArgs e) {
            // call methods to clean up
            Console.WriteLine("exiting...");
            abortpressed = true;
            WinInterop.UnregisterHotKey(IntPtr.Zero, 0);
            WinInterop.UnregisterHotKey(IntPtr.Zero, 1);
            WinInterop.UnregisterHotKey(IntPtr.Zero, 2);
            System.Environment.Exit(0);
        };
        Console.WriteLine("ctrl+c to exit");

        //main loop
        string message = "startorsplit";
        while (!abortpressed)
        {

            
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
            }
        }
    }

}



static partial class WinInterop
{

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
}
