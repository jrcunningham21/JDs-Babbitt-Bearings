using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace JDsWebApp.Areas.Services
{
    public class CookieHelper
    {
        public CookieHelper()
        {
        }

        public void SetCookie(string name, string value)
        {
            HttpCookie myCookie = new HttpCookie(name);
            DateTime now = DateTime.Now;
            myCookie.Value = value;
            //myCookie.Expires = now.AddYears(50);
            HttpContext.Current.Response.Cookies.Add(myCookie);
        }

        public string GetCookie(string name)
        {
            HttpCookie myCookie = HttpContext.Current.Request.Cookies[name];

            if (myCookie != null)
            {
                return myCookie.Value;
            }

            return string.Empty;
        }

        public void DeleteCookies()
        {
            HttpCookie aCookie;
            string cookieName;
            int limit = HttpContext.Current.Request.Cookies.Count;
            for (int i = 0; i < limit; i++)
            {
                cookieName = HttpContext.Current.Request.Cookies[i].Name;
                aCookie = new HttpCookie(cookieName);
                //aCookie.Expires = DateTime.Now.AddDays(-1); // make it expire yesterday
                HttpContext.Current.Response.Cookies.Add(aCookie); // overwrite it
            }
        }
    }
}
