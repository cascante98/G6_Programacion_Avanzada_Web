using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using TicketEvents.Entities.Models;
using TicketEvents.Repository.Context;
using TicketEvents;


namespace TicketEvents
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.CreatePerOwinContext<TicketEventsContext>(TicketEventsContext.Create);
            app.CreatePerOwinContext<UserManager<Usuario>>(CreateUserManager);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
            });
        }

        public UserManager<Usuario> CreateUserManager()
        {
            var manager = new UserManager<Usuario>(new UserStore<Usuario>(new TicketEventsContext()));
            manager.UserValidator = new UserValidator<Usuario>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            return manager;
        }
    }
}