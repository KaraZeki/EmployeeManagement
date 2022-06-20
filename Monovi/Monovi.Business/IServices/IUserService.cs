using Monovi.DataAccess.Repository;
using Monovi.Model.Concrete;
using Monovi.Model.Model;
using System.Collections.Generic;

namespace Monovi.Business.IServices
{
    public interface IUserService
    {
        public UserReturnModel UserAccountControl(UserViewModel userViewModel);

        public List<User> GetUserList();
        public User UpdateUser(User user);
        public void DeleteUser(int id );

        public string GetUserRole();
        public User GetUser(int id);
        public User AddUser(User  user);
    }
}
