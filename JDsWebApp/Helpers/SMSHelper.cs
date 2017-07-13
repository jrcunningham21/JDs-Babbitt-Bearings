using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JDsDataModel;
using Twilio;
using NLog;
using System.Web.Configuration;

namespace JDsWebApp.Helpers
{
    public static class SMSHelper
    {
        private readonly static Logger _logger = LogManager.GetCurrentClassLogger();
        public static void SendSMSToManagers(string message)
        {
            //Save API keys in better place?            
            var client = new TwilioRestClient(WebConfigurationManager.AppSettings["TwilioSid"], WebConfigurationManager.AppSettings["TwilioToken"]);
            var from = WebConfigurationManager.AppSettings["TwilioPhone"];           

            using (var db = new JDsDBEntities())
            {
                var managers = db.Employees.Where(x => x.Skills.OrderBy(s=> s.SkillId).FirstOrDefault().SkillId == 1).ToList();
                foreach(var manager in managers)
                {
                    //If it looks like a phone number
                    if (manager.Phone.Length > 8)
                    {
                        //Convert Phone Number
                        var to = ConvertToE164(manager.Phone);

                        //Send Message
                        var msg = client.SendMessage(from, to, message);

                        //Log Result
                        LogSMSAttempt(msg);                        
                    }
                }
            }
        }

        private static void LogSMSAttempt(Message msg)
        {
            if (msg.RestException == null)
                _logger.Trace("An SMS was sent to {0} saying {1}, it cost {2}.", msg.To, msg.Body, msg.Price);
            else
                _logger.Trace("SMS Error code: {0}, Error Message: {1", msg.RestException.Code, msg.RestException.Message);
        }

        private static string ConvertToE164(string phoneNumber)
        {
            string to = "+1";
            foreach (char num in phoneNumber)
            {
                if (Char.IsDigit(num))
                    to += num;
            }
            return to;
        } 
    }
}

