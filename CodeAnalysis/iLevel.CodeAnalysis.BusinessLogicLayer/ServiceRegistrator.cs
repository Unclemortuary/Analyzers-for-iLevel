using CodeAnalysis.BusinessLogicLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
