using System.Collections.Generic;

namespace iLevel.CodeAnalysis.BestPractices.Common
{
    public static class Constants
    {
        public const string IServiceCollectionNamespace = "Microsoft.Extensions.DependencyInjection.IServiceCollection";

        public static IReadOnlyList<string> NotSingletonAddServiceMethodNames = new List<string> { "AddScoped", "AddTransient" };
    }
}
