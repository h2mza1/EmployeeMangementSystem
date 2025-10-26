using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
namespace WebApplication1.Filters
{

    public class AllowCrossSiteJsonAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.AddHeader("Access-Control-Allow-Origin", "http://localhost:4200");
            filterContext.HttpContext.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
            filterContext.HttpContext.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept");

            base.OnActionExecuting(filterContext);
        }
    }

}
