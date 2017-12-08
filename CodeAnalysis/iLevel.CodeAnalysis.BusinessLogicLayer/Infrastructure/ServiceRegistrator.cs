using iLevel.CodeAnalysis.BusinessLogicLayer.CustomFactories;
using iLevel.CodeAnalysis.BusinessLogicLayer.CommonInterfaces;
using iLevel.CodeAnalysis.BusinessLogicLayer.Providers;
using Unity;

namespace iLevel.CodeAnalysis.BusinessLogicLayer.Infrastructure
{
    public class ServiceRegistrator
    {
        public static void RegisterServices(IUnityContainer container)
        {
            container.RegisterType<IDiagnosticProvider, DiagnosticProvider>();
            container.RegisterType<ISolutionProvider, SolutionProvider>();
            container.RegisterType<ICustomSyntaxFactory, CustomSyntaxFactory>();
            container.RegisterType<ICustomSolutionFactory, CustomSolutionFactory>();
        }
    }
}
