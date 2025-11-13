using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace EmployeeApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var cors = new EnableCorsAttribute(
                origins: "http://localhost:4200",
                headers: "*",
                methods: "*"
            );
            config.EnableCors(cors);

            config.MessageHandlers.Add(new PreflightRequestsHandler());

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional, action = RouteParameter.Optional }
            );

            config.Formatters.Remove(config.Formatters.XmlFormatter);
        }
    }

    
    public class PreflightRequestsHandler : DelegatingHandler
    {
        protected override async System.Threading.Tasks.Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            if (request.Method == HttpMethod.Options)
            {
                var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
                response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:4200");
                response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
                response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
                return response;
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
