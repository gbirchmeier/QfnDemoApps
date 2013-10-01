using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using QuickFix;
using QuickFix.Fields;

namespace PilotInititiator44
{
    public class PilotInit44App : QuickFix.MessageCracker, QuickFix.IApplication
    {
        Session _session = null;


        #region Command-line UI stuff

        private void Puts(string s)
        {
            Console.Write("\n>>> " + s + "\n_? ");
        }

        public void Run()
        {
            while (true)
            {
                try
                {
                    char action = QueryAction();
                    if (action == '1')
                        TransmitNewOrderSingle();
                    else if (action == 'q' || action == 'Q')
                        break;
                }
                catch (System.Exception e)
                {
                    string s = "Message Not Sent: " + e.Message;
                    s += "\nStackTrace: " + e.StackTrace;
                    Puts(s);
                }
            }
            Console.WriteLine("Program shutdown.  (There may be a delay while logouts are exchanged.)");
        }

        private char QueryAction()
        {
            Puts("==MENU==\n"
                + "1) Send a nonsense NewOrderSingle\n"
                + "Q) Quit"
            );

            HashSet<string> validActions = new HashSet<string>("1,q,Q".Split(','));

            string cmd = Console.ReadLine().Trim();
            if (cmd.Length != 1 || validActions.Contains(cmd) == false)
                throw new System.Exception("Invalid action");

            return cmd.ToCharArray()[0];
        }

        #endregion



        #region IApplication interface overrides

        public void OnCreate(SessionID sessionID)
        {
            _session = Session.LookupSession(sessionID);
        }

        public void OnLogon(SessionID sessionID) { Puts("Logon - " + sessionID.ToString()); }
        public void OnLogout(SessionID sessionID) { Puts("Logout - " + sessionID.ToString()); }

        public void FromAdmin(Message message, SessionID sessionID) { Puts("Received admin message, type " + message.Header.GetField(Tags.MsgType)); }
        public void ToAdmin(Message message, SessionID sessionID) { }

        public void FromApp(Message message, SessionID sessionID)
        {
            try
            {
                Crack(message, sessionID);
            }
            catch (Exception ex)
            {
                string s = "==Cracker exception==\n";
                s += ex.ToString() + "\n";
                s += ex.StackTrace;
                Puts(s);
            }
        }

        public void ToApp(Message message, SessionID sessionID)
        {
            /*
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
             */
        }
        #endregion


        #region MessageCracker handlers
        public void OnMessage(QuickFix.FIX44.ExecutionReport m, SessionID s)
        {
            Puts("Received execution report");
        }

        #endregion




        private void SendMessage(Message m)
        {
            if (_session != null)
                _session.Send(m);
            else
                Puts("Can't send message: session not created.");  // This probably won't ever happen.
        }


        private void TransmitNewOrderSingle()
        {
            QuickFix.FIX44.NewOrderSingle msg = new QuickFix.FIX44.NewOrderSingle(
                new ClOrdID("woooot"),
                new Symbol("IBM"),
                new Side(Side.BUY),
                new TransactTime(DateTime.Now),
                new OrdType(OrdType.MARKET));

            msg.Set(new OrderQty(99));
           
            SendMessage(msg);
        }

    }
}
