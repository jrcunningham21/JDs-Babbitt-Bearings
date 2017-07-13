using System.Web;
using System.Web.Mvc;

namespace JD_s_Babbitt_Bearings
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}