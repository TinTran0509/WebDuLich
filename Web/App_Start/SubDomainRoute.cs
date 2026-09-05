using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using Web.BaseSecurity;
using Web.Core;
using Web.Model;
using Web.Repository;
using Web.Repository.Entity;

namespace Web.App_Start
{
    public class SubDomainRoute : Route
    {
        private readonly string[] namespaces;
        public SubDomainRoute(string url, object defaults, string[] namespaces)
            : base(url, new RouteValueDictionary(defaults), new MvcRouteHandler())
        {
            this.namespaces = namespaces;
        }
        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            return null;
        }
    }
}