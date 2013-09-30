using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using QuickFix;
using QuickFix.Fields;

namespace PilotAcceptor44
{
    public class PilotAcc44App : QuickFix.MessageCracker, QuickFix.IApplication
    {
        static readonly decimal DEFAULT_MARKET_PRICE = 10;

        int orderID = 0;
        int execID = 0;

        private string GenOrderID() { return (++orderID).ToString(); }
        private string GenExecID() { return (++execID).ToString(); }

        #region QuickFix.Application Methods

        public void FromApp(Message message, SessionID sessionID)
        {
            Crack(message, sessionID);
        }

        public void ToApp(Message message, SessionID sessionID)
        {
            Console.WriteLine("OUT: " + message);
        }

        public void FromAdmin(Message message, SessionID sessionID) { }
        public void OnCreate(SessionID sessionID) { }
        public void OnLogout(SessionID sessionID) { }
        public void OnLogon(SessionID sessionID) { }
        public void ToAdmin(Message message, SessionID sessionID) { }
        #endregion

        #region MessageCracker overloads

        public void OnMessage(QuickFix.FIX44.NewOrderSingle n, SessionID s)
        {
            Console.WriteLine("* Got a NewOrderSingle.  Responding with an ExecutionReport.");

            Symbol symbol = n.Symbol;
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

            QuickFix.FIX44.ExecutionReport exReport = new QuickFix.FIX44.ExecutionReport(
                new OrderID(GenOrderID()),
                new ExecID(GenExecID()),
                new ExecType(ExecType.FILL),
                new OrdStatus(OrdStatus.FILLED),
                symbol, //shouldn't be here?
                side,
                new LeavesQty(0),
                new CumQty(orderQty.getValue()),
                new AvgPx(price.getValue()));

            exReport.Set(clOrdID);
            exReport.Set(symbol);
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
                Console.WriteLine("==session not found exception!==");
                Console.WriteLine(ex.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void OnMessage(QuickFix.FIX44.News n, SessionID s)
        {
            Console.WriteLine("* Got a News.  Headline: " + n.Headline.getValue());
        }

        public void OnMessage(QuickFix.FIX44.BusinessMessageReject n, SessionID s)
        {
            Console.WriteLine("***The initiator rejected one of my messages.");
        }
        
        #endregion //MessageCracker overloads
    }
}
