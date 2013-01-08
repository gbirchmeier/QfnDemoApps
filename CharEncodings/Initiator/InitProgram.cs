using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickFix;

namespace Initiator
{
    class InitProgram
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("== CharEncoding Initiator startup");

            try
            {
                SessionSettings settings = new QuickFix.SessionSettings("initiator.cfg");
                CEInitiatorApp application = new CEInitiatorApp();
                IMessageStoreFactory storeFactory = new QuickFix.FileStoreFactory(settings);
                ILogFactory logFactory = new QuickFix.ScreenLogFactory(settings);
                QuickFix.Transport.SocketInitiator initiator = new QuickFix.Transport.SocketInitiator(application, storeFactory, settings, logFactory);
                
                initiator.Start();
                application.ListenOnConsole();
                initiator.Stop();
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            Environment.Exit(1);
        }
    }
}
