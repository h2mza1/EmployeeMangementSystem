//using System;
//using System.Net;
//using System.Net.Http;
//using System.Web.Http.Controllers;
//using System.Web.Http.Filters;

//namespace WebApplication1.Filters
//{
//    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
//    public class AllowCrossSiteJsonAttribute : ActionFilterAttribute
//    {
//        public override void OnActionExecuting(HttpActionContext actionContext)
//        {
//            var headers = actionContext.Request.Headers;

//            // إضافة الـ headers للـ response
//            actionContext.Response = actionContext.Response ?? actionContext.Request.CreateResponse(HttpStatusCode.OK);
//            actionContext.Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:4200");
//            actionContext.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
//            actionContext.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");

//            // إذا كان preflight request (OPTIONS) نوقف التنفيذ
//            if (actionContext.Request.Method == HttpMethod.Options)
//            {
//                actionContext.Response.StatusCode = HttpStatusCode.OK;
//            }

//            base.OnActionExecuting(actionContext);
//        }
//    }
//}
