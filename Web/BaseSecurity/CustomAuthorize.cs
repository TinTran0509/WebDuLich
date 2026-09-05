using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.BaseSecurity
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class CustomAuthorize: AuthorizeAttribute
    {
        public CustomAuthorize(params object[] roles)
        {
            if (roles.Any(r => r.GetType().BaseType != typeof(Enum)))
                throw new ArgumentException("roles");
            //this.Roles = string.Join(",", roles.Select(r => Enum.GetName(r.GetType(), r)));
            this.Roles = string.Join(",", roles.Select(r => (int)r));
        }
    }
}