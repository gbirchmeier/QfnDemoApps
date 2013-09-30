using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using QuickFix;
using QuickFix.Transport;

namespace PilotInititiator44
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("=============");
            Console.WriteLine("This is only an example program, meant to run against the Executor or SimpleAcceptor example programs.");
            Console.WriteLine();
            Console.WriteLine("                                                    ! ! !");
            Console.WriteLine("              DO NOT USE THIS ON A COMMERCIAL FIX INTERFACE!  It won't work and it's a bad idea!");
            Console.WriteLine("                                                    ! ! !");
            Console.WriteLine();
            Console.WriteLine("=============");

            if (args.Length != 1)
            {
                System.Console.WriteLine("usage: TradeClient.exe CONFIG_FILENAME");
                System.Environment.Exit(2);
            }

            string file = args[0];

            try
            {
                QuickFix.SessionSettings settings = new QuickFix.SessionSettings(file);
                PilotInit44App myApp = new PilotInit44App();
                QuickFix.IMessageStoreFactory storeFactory = new QuickFix.FileStoreFactory(settings);
                QuickFix.ILogFactory logFactory = new QuickFix.ScreenLogFactory(settings);
                QuickFix.Transport.SocketInitiator initiator = new QuickFix.Transport.SocketInitiator(myApp, storeFactory, settings, logFactory);

                initiator.Start();
                myApp.Run();
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            Environment.Exit(0);
        }
    }
}
