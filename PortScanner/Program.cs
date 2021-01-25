using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace PortScanner
{
    class Program
    {
        private static string host = "N/a";
        private static int openPorts = 0;
        private static int closedPorts = 0;
        private static List<string> open = new List<string>();

        static void Main(string[] args)
        {
            Console.Title = "CSharp Port Scanner";
            main();
            Console.ReadKey();
        }

        static void main() 
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine("      ___           _     __                      ========================");
            Console.WriteLine("     / _ \\___  _ __| |_  / _\\ ___ __ _ _ __       = * Last Scan Results  ");
            Console.WriteLine("    / /_)/ _ \\| '__| __| \\ \\ / __/ _` | '_ \\      = * Open Ports -> {0}", openPorts);
            Console.WriteLine("   / ___/ (_) | |  | |_  _\\ \\ (_| (_| | | | |     = * Closed Ports -> {0}", closedPorts);
            Console.WriteLine("   \\/    \\___/|_|   \\__| \\__/\\___\\__,_|_| |_|     = * Open -> ( {0} )", String.Join(", ", open));
            Console.WriteLine("                                                  = * Host -> {0}", host);
            Console.WriteLine("                                                  ========================");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("");
            Console.Write("Enter a target: ");
            host = Console.ReadLine();
            Console.Write("Enter a start port: ");
            string startPort = Console.ReadLine();
            Console.Write("Enter a end port: ");
            string endPort = Console.ReadLine();
            Console.Write("Enter a wait time: ");
            string waitTime = Console.ReadLine();
            Console.WriteLine();

            if (string.IsNullOrWhiteSpace(host))
            {
                error(2, "Missing target!");
                Thread.Sleep(700);
                Console.Clear();
                main();
            }
            else if (string.IsNullOrWhiteSpace(startPort))
            {
                error(2, "Missing start port!");
                Thread.Sleep(700);
                Console.Clear();
                main();
            }
            else if (string.IsNullOrWhiteSpace(endPort))
            {
                error(2, "Missing end port!");
                Thread.Sleep(700);
                Console.Clear();
                main();
            }
            else if (string.IsNullOrWhiteSpace(waitTime))
            {
                error(2, "Missing wait time!");
                Thread.Sleep(700);
                Console.Clear();
                main();
            }
            if (Convert.ToInt32(waitTime) < 40) 
            {
                error(1, $"Anything below 40ms for a wait could make any results inaccurate. Are you sure you want to use {waitTime}ms?");
                Console.WriteLine("Please choose yes or no.");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(" - 1. Yes");
                Console.WriteLine(" - 2. No ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Option: ");
                string option = Console.ReadLine();

                if (option == "2")
                {
                    Console.Clear();
                    main();
                }
            }
                

            openPorts = 0;
            closedPorts = 0;
            open.Clear();

            ping(IPAddress.Parse(host), Convert.ToInt32(startPort), Convert.ToInt32(endPort), Convert.ToInt32(waitTime));
        }

        static void error(int type, string message)
        {
            switch (type)
            {
                case 0:
                    // Information
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"NOTICE: {message}");
                    break;
                case 1:
                    // Warning
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"WARNING: {message}");
                    break;
                case 2:
                    // Error
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"ERROR: {message}");
                    break;
                case 3:
                    // Success
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"SUCCESS: {message}");
                    break;
            }
            Console.ForegroundColor = ConsoleColor.White;
            Thread.Sleep(300);
        }

        static void ping(IPAddress ip, int startPort, int endPort, int waitTime)
        {
            Console.WriteLine($"Port Scan Started ( {DateTime.Now.ToString("h:mm:ss")} )");

            for (int i = startPort; i < endPort+1; i++)
            {
                using (TcpClient client = new TcpClient())
                {
                    if (client.ConnectAsync(ip, i).Wait(waitTime))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\r{0} is open  ", i);
                        openPorts += 1;
                        open.Add(i.ToString());
                        client.Dispose();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("\r{0} is closed", i);
                        closedPorts += 1;
                        client.Dispose();
                    }
                }
            }

            Console.WriteLine();
            error(0, "Scan Finished!");
            Console.WriteLine("Refreshing...");
            Thread.Sleep(500);
            Console.Clear();
            main();
        }
    }
}
