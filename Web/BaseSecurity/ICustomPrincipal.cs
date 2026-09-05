using System.Collections.Generic;
using System.Security.Principal;
using Web.Model;

namespace Web.BaseSecurity
{
    public interface ICustomPrincipal : IPrincipal
    {
        int ID { get; set; }
        string Username { get; set; }
        string FullName { get; set; }
        string Email { get; set; }
        int UserType { get; set; }
        string GroupUser { get; set; }
        bool isAdmin { get; set; }
    }
}