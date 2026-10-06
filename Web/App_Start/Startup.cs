using Hangfire;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Service;

namespace Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalConfiguration.Configuration
                .UseSqlServerStorage("DefaultConnection");

            app.UseHangfireServer();

            app.UseHangfireDashboard();

            RecurringJob.AddOrUpdate<BookingEmailJob>(
                "booking-email-job",
                x => x.Execute(),
                "*/5 * * * *"
            );
        }
    }
}