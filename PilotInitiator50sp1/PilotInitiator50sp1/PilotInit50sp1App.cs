using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using QuickFix;
using QuickFix.Fields;

namespace PilotInitiator50sp1
{
    public class PilotInit50sp1App : QuickFix.MessageCracker, QuickFix.IApplication
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
                    else if (action == '2')
                        TransmitAdvertisement();
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
                + "1) Send a nonsense NewOrderSingle (35=D), so we get a ExecReport in response\n"
                + "2) Send a nonsense Advertisement (35=7), so we get a rejection\n"
                + "Q) Quit"
            );

            HashSet<string> validActions = new HashSet<string>("1,2,q,Q".Split(','));

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

        public void ToAdmin(Message message, SessionID sessionID)
        {
            if (message.Header.GetField(Tags.MsgType) == MsgType.LOGON)
            {
                message.SetField(new QuickFix.Fields.Username("batman"));
                message.SetField(new QuickFix.Fields.Password("gotham123"));
            }
        }

        public void ToApp(Message message, SessionID sessionID) { }

        public void FromApp(Message message, SessionID sessionID)
        {
            try
            {
                Crack(message, sessionID);
            }
            catch (Exception ex)
            {
                string s = "==Cracker exception==\n";
                s += ex.ToString();
                Puts(s);
            }
        }

        #endregion


        #region MessageCracker handlers
        public void OnMessage(QuickFix.FIX50SP1.ExecutionReport m, SessionID s)
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
            QuickFix.FIX50SP1.NewOrderSingle msg = new QuickFix.FIX50SP1.NewOrderSingle(
                new ClOrdID("woooot"),
                new Side(Side.BUY),
                new TransactTime(DateTime.Now),
                new OrdType(OrdType.MARKET));

            msg.Set(new OrderQty(99));

            SendMessage(msg);
        }

        private void TransmitAdvertisement()
        {
            QuickFix.FIX50SP1.Advertisement msg = new QuickFix.FIX50SP1.Advertisement(
                new AdvId("spam"),
                new AdvTransType("luncheon meat"),
                new AdvSide(AdvSide.SELL),
                new Quantity(1000000));

            SendMessage(msg);
        }
    }
}
