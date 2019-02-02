using System.Web.Mvc;

namespace Hadis.Areas.CustomerArea
{
    public class CustomerAreaAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "CustomerArea";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "CustomerArea_default",
                "CustomerArea/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Hadis.Areas.CustomerArea.Controller" }
            );
        }
    }
}