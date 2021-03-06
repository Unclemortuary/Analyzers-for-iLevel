﻿using System.Web.Mvc;
using AAL = iLevel.CodeAnalysis.AnalyzersAccesLayer.Infrastructure;
using CodeAnalysisService.Infrastructure;
using Unity;
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
            AAL.ServiceRegistrator.RegisterServices(_container);
            _container.RegisterType<IMapper, Mapper>();
            DependencyResolver.SetResolver(new UnityDependencyResolver(_container));
        }
    }
}