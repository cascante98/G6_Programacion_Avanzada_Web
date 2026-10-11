using System.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Dependencies;
using TicketEvents.Controllers.Api;
using TicketEvents.Repository.Context;
using TicketEvents.Repository.Repositories;
using TicketEvents.Services.Services;
using Unity;
using Unity.Mvc5;

namespace TicketEvents
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            container.RegisterType<TicketEventsContext>();

            container.RegisterType<IEventoRepository, EventoRepository>();
            container.RegisterType<IEventoService, EventoService>();

            container.RegisterType<IOrdenRepository, OrdenRepository>();
            container.RegisterType<IOrdenService, OrdenService>();

            container.RegisterType<IUsuarioRepository, UsuarioRepository>();
            container.RegisterType<IUsuarioService, UsuarioService>();
            container.RegisterType<IResenaRepository, ResenaRepository>();
            container.RegisterType<IResenaService, ResenaService>();
            container.RegisterType<IDashboardRepository, DashboardRepository>();
            container.RegisterType<IDashboardService, DashboardService>();

            container.RegisterType<DashboardApiController>();
            container.RegisterType<ComprasApiController>();
            container.RegisterType<ResenasApiController>();
            container.RegisterType<CarritoApiController>();
            container.RegisterType<EventosApiController>();

            System.Web.Mvc.DependencyResolver.SetResolver(new UnityDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new UnityWebApiDependencyResolver(container);
        }
    }

    public sealed class UnityWebApiDependencyResolver : System.Web.Http.Dependencies.IDependencyResolver
    {
        private readonly IUnityContainer _container;
        public UnityWebApiDependencyResolver(IUnityContainer container) { _container = container; }
        public IDependencyScope BeginScope() { return new UnityWebApiDependencyResolver(_container.CreateChildContainer()); }
        public object GetService(Type serviceType)
        {
            try { return _container.IsRegistered(serviceType) || !serviceType.IsAbstract ? _container.Resolve(serviceType) : null; }
            catch { return null; }
        }
        public IEnumerable<object> GetServices(Type serviceType)
        {
            try { return _container.ResolveAll(serviceType); }
            catch { return Enumerable.Empty<object>(); }
        }
        public void Dispose() { _container.Dispose(); }
    }
}
