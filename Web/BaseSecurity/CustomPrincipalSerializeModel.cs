using System.Collections.Generic;
using Web.Model;

namespace Web.BaseSecurity
{
    public class CustomPrincipalSerializeModel
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int UserType { get; set; }
        public string GroupUser { get; set; }
        public byte? NoiBo { get; set; }
        public bool isAdmin { get; set; }
    }
}