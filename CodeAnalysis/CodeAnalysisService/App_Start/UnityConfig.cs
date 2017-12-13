using Unity;
using System.Web.Mvc;
using iLevel.CodeAnalysis.BusinessLogicLayer.Infrastructure;
using Unity.Mvc5;

namespace CodeAnalysisService
{
    public static class UnityConfig
    {
        private static UnityContainer _container;

        public static UnityContainer Container => _container;

        public static void RegisterServices()
        {
            _container = new UnityContainer();
            ServiceRegistrator.RegisterServices(_container);
            DependencyResolver.SetResolver(new UnityDependencyResolver(_container));
        }
    }
}