using iLevel.CodeAnalysis.AnalyzersAccesLayer.Interfaces;
using iLevel.CodeAnalysis.AnalyzersAccesLayer.CustomFactories;
using Unity;

namespace iLevel.CodeAnalysis.AnalyzersAccesLayer.Infrastructure
{
    public class ServiceRegistrator
    {
        public static void RegisterServices(IUnityContainer container)
        {
            container.RegisterType<ISyntaxFactory, CustomSyntaxFactory>();
            container.RegisterType<IDiagnosticProvider, DiagnosticProvider>();
            container.RegisterType<IMapper, Mapper>();
        }
    }
}
