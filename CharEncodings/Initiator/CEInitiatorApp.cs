using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickFix;

namespace Initiator
{
    public class CEInitiatorApp : QuickFix.MessageCracker, QuickFix.IApplication
    {
        Session _session = null;


        #region IApplication interface overrides

        public void OnCreate(SessionID sessionID)
        {
            _session = Session.LookupSession(sessionID);
        }

        public void OnLogon(SessionID sessionID) { Console.WriteLine("Logon - " + sessionID.ToString()); }
        public void OnLogout(SessionID sessionID) { Console.WriteLine("Logout - " + sessionID.ToString()); }

        public void FromAdmin(Message message, SessionID sessionID) { }
        public void ToAdmin(Message message, SessionID sessionID) { }

        public void FromApp(Message message, SessionID sessionID)
        {
            Console.WriteLine("IN:  " + message.ToString());
            try
            {
                Crack(message, sessionID);
            }
            catch (Exception ex)
            {
                Console.WriteLine("==Cracker exception==");
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }
        }

        public void ToApp(Message message, SessionID sessionID)
        {
            try
            {
                bool possDupFlag = false;
                if (message.Header.IsSetField(QuickFix.Fields.Tags.PossDupFlag))
                {
                    possDupFlag = QuickFix.Fields.Converters.BoolConverter.Convert(
                        message.Header.GetField(QuickFix.Fields.Tags.PossDupFlag)); /// FIXME
                }
                if (possDupFlag)
                    throw new DoNotSend();
            }
            catch (FieldNotFoundException)
            { }

            Console.WriteLine();
            Console.WriteLine("OUT: " + message.ToString());
        }
        #endregion


        public void ListenOnConsole()
        {
            while (true)
            {
                try
                {
                    char action = QueryAction();
                    if (action == '1')
                        SendNewsWithUtf();
                    else if (action == '2')
                        SendNewsWithoutUtf();
                    else if (action == '3')
                        break;
                }
                catch (System.Exception e)
                {
                    Console.WriteLine("Message Not Sent: " + e.Message);
                    Console.WriteLine("StackTrace: " + e.StackTrace);
                }
            }
            Console.WriteLine("Program shutdown.");
        }

        private char QueryAction()
        {
            Console.Write("\n"
                + "1) Send news with utf character\n"
                + "2) Send news without utf character\n"
                + "3) Quit\n"
                + "Action: "
            );

            string line = Console.ReadLine().Trim();
            if (line.Length != 1)
                throw new System.Exception("Invalid action");

            char val = line[0];
            switch (val)
            {
                case '1':
                case '2':
                case '3': break;
                default: throw new System.Exception("Invalid action");
            }
            return val;
        }

        private void SendNewsWithUtf()
        {
            QuickFix.FIX44.News n = new QuickFix.FIX44.News();
            n.Header.SetField(new QuickFix.Fields.MessageEncoding("UTF-8"));
            n.Headline = new QuickFix.Fields.Headline("here's some utf: ole");
            n.EncodedHeadline = new QuickFix.Fields.EncodedHeadline("here's some utf: olé");
            Session.SendToTarget(n, _session.SessionID);
        }

        private void SendNewsWithoutUtf()
        {
            QuickFix.FIX44.News n = new QuickFix.FIX44.News();
            n.Headline = new QuickFix.Fields.Headline("it's all ascii here, bro");
            Session.SendToTarget(n, _session.SessionID);
        }
    }
}
