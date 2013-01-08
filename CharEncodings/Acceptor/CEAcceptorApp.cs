using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickFix;

namespace Acceptor
{
    public class CEAcceptorApp : QuickFix.MessageCracker, QuickFix.IApplication
    {
        #region QuickFix.Application Methods

        public void FromApp(Message message, SessionID sessionID)
        {
            Console.WriteLine("IN:  " + message);
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

        public void OnMessage(QuickFix.FIX44.News msg, SessionID sessionId)
        {
            //byte[] ole = Encoding.UTF8.GetBytes("olé");
            QuickFix.FIX44.News response = new QuickFix.FIX44.News();
            response.Header.SetField(new QuickFix.Fields.MessageEncoding("UTF-8"));
            response.Headline = new QuickFix.Fields.Headline("ole");
            response.EncodedHeadline = new QuickFix.Fields.EncodedHeadline("olé");
            Session.SendToTarget(response, sessionId);
        }
    }
}
