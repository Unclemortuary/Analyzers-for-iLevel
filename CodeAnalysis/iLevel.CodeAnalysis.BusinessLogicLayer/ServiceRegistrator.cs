using iLevel.CodeAnalysis.BusinessLogicLayer.CustomFactories;
using iLevel.CodeAnalysis.BusinessLogicLayer.CommonInterfaces;
using Unity;


namespace iLevel.CodeAnalysis.BusinessLogicLayer
{
    public class ServiceRegistrator
    {
        public static void RegisterServices(IUnityContainer container)
        {
            container.RegisterType<IDiagnosticService, DiagnosticBLL>();
            container.RegisterType<ISolutionCreator, SolutionBLL>();
            container.RegisterType<ICustomSyntaxFactory, CustomSyntaxFactory>();
        }
    }
}
