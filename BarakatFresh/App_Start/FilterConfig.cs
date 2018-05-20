using BarakatFresh.WebSecurity;
using System.Web;
using System.Web.Mvc;

namespace testcart
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());

            var attr = new InitializeSimpleMembershipAttribute();
            // here is the important part
            attr.OnActionExecuting(new ActionExecutingContext());
            filters.Add(attr);

        }
    }
}
