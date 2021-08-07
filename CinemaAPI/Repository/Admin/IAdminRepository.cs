using CinemaAPI.Models;
using CinemaAPI.ModelsView;
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
    }
}
