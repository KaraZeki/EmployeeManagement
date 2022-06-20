using System.Collections.Generic;

namespace Monovi.Model.Concrete
{
    public class Role : BaseEntity
    {
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }

        public ICollection<User> Users { get; set; }
      
    }
}
