using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Optimization;
using CodeAnalysisService.Infrastructure;
using Unity;

namespace CodeAnalysisService
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            AnalyzerConfig.RegisterAnalyzers(AnalyzerProvider.Analyzers);
            UnityConfig.RegisterServices();
        }
    }
}
