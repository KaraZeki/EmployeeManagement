using Microsoft.AspNetCore.Http;
using Monovi.Business.IServices;
using Monovi.DataAccess.Repository;
using Monovi.Model.Concrete;
using Monovi.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Monovi.Business.Services
{
    internal class UserService : IUserService
    {

        private readonly IGenericRepository<User> _userRepository;
        private readonly IGenericRepository<Role> _roleRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserService(IGenericRepository<User> userRepository, IHttpContextAccessor httpContextAccessor, IGenericRepository<Role> roleRepository)
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
            _roleRepository = roleRepository;
        }

        public User AddUser(User user)
        {
            user.RoleId = 3;
          return  _userRepository.Add(user);
        }

        public void DeleteUser(int id)
        {
            var user = _userRepository.Get(x => x.Id == id).FirstOrDefault();
            _userRepository.Delete(user);
        }

        public User GetUser(int id)
        {
            return _userRepository.Get(x => x.Id == id).FirstOrDefault();
        }

        public List<User> GetUserList()
        {
            return  _userRepository.GetAll().ToList();           
        }

        public string GetUserRole()
        {
            var claims = _httpContextAccessor.HttpContext.User.Claims.ToList();
            var id = claims.Where(x => x.Type == "Id").Select(x=>x.Value).FirstOrDefault();
            var roleId = _userRepository.Get(x => x.Id == int.Parse(id)).FirstOrDefault().RoleId;
            var role = _roleRepository.Get(x => x.Id == roleId).FirstOrDefault();
            return role.RoleName;
        }

        public User UpdateUser(User user)
        {
            user.RoleId = 3;
           return _userRepository.Update(user);
        }

        public UserReturnModel UserAccountControl(UserViewModel userViewModel)
        {
            try
            {
                var userReturn = new UserReturnModel();
                var hasEmail = _userRepository.Get(x => x.Email == userViewModel.Email).FirstOrDefault();
                if (hasEmail == null)
                {
                    userReturn.IsOk = false;
                    userReturn.Message = "Email has not correct";
                }
                else
                {
                    var hasPassword = _userRepository.Get(x => x.Password == userViewModel.Password&&x.Email==userViewModel.Email).FirstOrDefault();
                    if (hasPassword == null)
                    {
                        userReturn.IsOk = false;
                        userReturn.Message = "Psssword has not correct";
                    }
                    else
                    {
                        userReturn.Id = hasPassword.Id;
                        userReturn.IsOk = true;
                        userReturn.Message = "Success";
                    }
                }

                return userReturn;

            }
            catch (Exception ex)
            {

                throw ex;
            }
            throw new NotImplementedException();
        }
    }
}
