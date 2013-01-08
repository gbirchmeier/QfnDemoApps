using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickFix;

namespace Acceptor
{
    class AccProgram
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("=== CharEncoding Acceptor startup");

            try
            {
                SessionSettings settings = new SessionSettings("acceptor.cfg");
                IApplication app = new CEAcceptorApp();
                IMessageStoreFactory storeFactory = new FileStoreFactory(settings);
                ILogFactory logFactory = new FileLogFactory(settings);
                ThreadedSocketAcceptor acceptor = new ThreadedSocketAcceptor(app, storeFactory, settings, logFactory);

                acceptor.Start();
                Console.WriteLine("press <enter> to quit");
                Console.Read();
                acceptor.Stop();
            }
            catch (System.Exception e)
            {
                Console.WriteLine("==FATAL ERROR==");
                Console.WriteLine(e.ToString());
            }
        }
    }
}
