using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using QuickFix;
using QuickFix.Fields;

namespace PilotAcceptor50sp1
{
    public class PilotAcc50sp1App : QuickFix.MessageCracker, QuickFix.IApplication
    {
        static readonly decimal DEFAULT_MARKET_PRICE = 10;

        int orderID = 0;
        int execID = 0;

        private string GenOrderID() { return (++orderID).ToString(); }
        private string GenExecID() { return (++execID).ToString(); }

        private void Puts(string s)
        {
            Console.Write("\n>>> " + s + "\n_? ");
        }

        #region QuickFix.Application Methods

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

        public void ToApp(Message message, SessionID sessionID)
        {
            Puts("OUT: " + message);
        }

        public void FromAdmin(Message message, SessionID sessionID) { }
        public void OnCreate(SessionID sessionID) { }
        public void OnLogout(SessionID sessionID)
        {
            Puts($"Logout: {sessionID.ToString()}");
        }
        public void OnLogon(SessionID sessionID)
        {
            Puts($"Logon: {sessionID.ToString()}");
        }
        public void ToAdmin(Message message, SessionID sessionID) { }
        #endregion

        #region MessageCracker overloads

        public void OnMessage(QuickFix.FIX50SP1.NewOrderSingle n, SessionID s)
        {
            Puts("Got a NewOrderSingle.  Responding with an ExecutionReport.");

            Side side = n.Side;
            OrdType ordType = n.OrdType;
            OrderQty orderQty = n.OrderQty;
            Price price = new Price(DEFAULT_MARKET_PRICE);
            ClOrdID clOrdID = n.ClOrdID;

            switch (ordType.getValue())
            {
                case OrdType.LIMIT:
                    price = n.Price;
                    if (price.Obj == 0)
                        throw new IncorrectTagValue(price.Tag);
                    break;
                case OrdType.MARKET: break;
                default: throw new IncorrectTagValue(ordType.Tag);
            }

            QuickFix.FIX50SP1.ExecutionReport exReport = new QuickFix.FIX50SP1.ExecutionReport(
                new OrderID(GenOrderID()),
                new ExecID(GenExecID()),
                new ExecType(ExecType.TRADE),
                new OrdStatus(OrdStatus.FILLED),
                side,
                new LeavesQty(0),
                new CumQty(orderQty.getValue()));

            exReport.Set(clOrdID);
            exReport.Set(orderQty);
            exReport.Set(new LastQty(orderQty.getValue()));
            exReport.Set(new LastPx(price.getValue()));

            if (n.IsSetAccount())
                exReport.SetField(n.Account);

            try
            {
                Session.SendToTarget(exReport, s);
            }
            catch (SessionNotFound ex)
            {
                Puts("==session not found exception!==");
                Puts(ex.ToString());
            }
            catch (Exception ex)
            {
                Puts(ex.ToString());
            }
        }

        public void OnMessage(QuickFix.FIX50SP1.News n, SessionID s)
        {
            Puts("Got a News.  Headline: " + n.Headline.getValue());
        }

        public void OnMessage(QuickFix.FIX50SP1.BusinessMessageReject n, SessionID s)
        {
            Puts("***The initiator rejected one of my messages.");
        }

        #endregion //MessageCracker overloads
    }
}
