using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickFix;
using QuickFix.Transport;

namespace PilotAcceptor50sp1
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("=============");
            Console.WriteLine("This is for evaluating new QF/n builds and features.");
            Console.WriteLine("It's designed to communicate with PilotInitiator50sp1.");
            Console.WriteLine("=============");

            if (args.Length != 1)
            {
                Console.WriteLine("usage: PilotInitiator50sp1 CONFIG_FILENAME");
                System.Environment.Exit(2);
            }

            try
            {
                SessionSettings settings = new SessionSettings(args[0]);
                IApplication myApp = new PilotAcc50sp1App();
                IMessageStoreFactory storeFactory = new FileStoreFactory(settings);
                ILogFactory logFactory = new FileLogFactory(settings);
                ThreadedSocketAcceptor acceptor = new ThreadedSocketAcceptor(myApp, storeFactory, settings, logFactory);

                acceptor.Start();
                while (true)
                {
                    Console.WriteLine("Enter 'q' to quit.  Enter 'p' to call stop().  Enter 'r' to call start().");

                    switch (Console.Read())
                    {
                        case 'p':
                            acceptor.Stop();
                            Console.WriteLine("Stopped.");
                            break;
                        case 'r':
                            acceptor.Start();
                            Console.WriteLine("Started.");
                            break;
                        case 'q':
                            acceptor.Stop();
                            Console.WriteLine("Terminated.");
                            return;
                    }
                    Console.ReadLine(); // flush it
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine("==FATAL ERROR==");
                Console.WriteLine(e.ToString());
            }
        }
    }
}
