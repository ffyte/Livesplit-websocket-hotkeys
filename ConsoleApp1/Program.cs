// See https://aka.ms/new-console-template for more information
using NonInvasiveKeyboardHookLibrary;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using Websocket.Client;

internal class Program
{
    private static void Main()
    {
        //startup and setting saving
        ConsoleKeyInfo startorsplit = new();
        ConsoleKeyInfo reset =new();
        ConsoleKeyInfo unsplit =new();
        string ip="127.0.0.1:16835";
        bool newmessage = false;
        string message="startorsplit";

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
        var keyboardHookManager = new KeyboardHookManager();
        keyboardHookManager.Start();
        Console.WriteLine("registering keys");
         keyboardHookManager.RegisterHotkey(0x4A, () =>
        {
            Console.WriteLine("start or split");
            newmessage = true;
            message = "startorsplit";
        });
        
        keyboardHookManager.RegisterHotkey(0x62, () =>
        {
            Console.WriteLine("reset");
            newmessage = true;
            message = "reset";
        });
        keyboardHookManager.RegisterHotkey(0x63, () =>
        {
            Console.WriteLine("unsplit");
            newmessage= true;
            message = "unsplit";
        });
        keyboardHookManager.RegisterHotkey(0x60, () =>
        {
            Console.WriteLine("NumPad0 detected");
            newmessage = true;
            message = "startorsplit";
        });

        //Console.ReadKey();


        //Communication to server
        var url = new Uri("ws://" + ip + "/livesplit");
        using var client = new WebsocketClient(url);
        Console.WriteLine("Connected to: " + url);
        client.Start();
        
        client.Send("startorsplit");

        Console.CancelKeyPress += delegate {
            // call methods to clean up
            client.Dispose();
        };
        Console.WriteLine("ctrl+c to exit");
        //main loop
        while (true)
        {
            if (newmessage)
            {
                Console.WriteLine(message);
                client.Send(message);
            }
        }
    }


}


