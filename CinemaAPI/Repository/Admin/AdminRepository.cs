using CinemaAPI.Models;
using CinemaAPI.ModelsView;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaAPI.Repository.Admin
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ApplicationDb db;
        private readonly UserManager<ApplicationUser> userManager;

        public AdminRepository(ApplicationDb _db, UserManager<ApplicationUser> _userManager)
        {
            db = _db;
            userManager = _userManager;
        }

        public async Task<ApplicationUser> AddUser(AddUserModel userModel)
        {
            if (userModel == null)
            {
                return null;
            }
            var user = new ApplicationUser
            {
                Email = userModel.Email,
                PasswordHash = userModel.Password,
                UserName = userModel.UserName,
                PhoneNumber = userModel.PhoneNumber,
                Country = userModel.Country,
            };
            var res = await userManager.CreateAsync(user, userModel.Password);
            if (res.Succeeded)
            {
                return user;
            }
            return null;
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsers()
        {
            return await db.Users.ToListAsync();
        }
    }
}
