using CinemaAPI.Models;
using CinemaAPI.ModelsView;
using CinemaAPI.ModelsView.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaAPI.Repository.Admin
{
   public interface IAdminRepository
    {
        public Task<IEnumerable<ApplicationUser>> GetUsers();
        public Task<ApplicationUser> AddUser(AddUserModel userModel);
        public Task<ApplicationUser> GetUser(string id);
        public Task<ApplicationUser> EditUserAsync(EditUserModel editUserModel);

        public Task<ApplicationUser> DeleteUser(string id);
        public Task<IEnumerable<UsersRolesModel>> GetRoleNameAsync();
        public Task<IEnumerable<ApplicationRole>> GetAllRoleAsync();




    }
}
