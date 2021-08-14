using CinemaAPI.Models;
using CinemaAPI.ModelsView;
using CinemaAPI.ModelsView.Users;
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
        private readonly RoleManager<ApplicationRole> roleManager;

        public AdminRepository(ApplicationDb _db, UserManager<ApplicationUser> _userManager,RoleManager<ApplicationRole> _roleManager)
        {
            db = _db;
            userManager = _userManager;
            roleManager = _roleManager;
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
                if (await roleManager.RoleExistsAsync("User"))
                {
                    if (!await userManager.IsInRoleAsync(user, "User") && !await userManager.IsInRoleAsync(user, "Admin"))
                    {
                        await userManager.AddToRoleAsync(user, "User");
                    }
                }

                    return user;
            }
            return null;
        }



        public async Task<ApplicationUser> GetUser(string id)
        {
            if (id==null)
            {
                return null;
            }
            var user = await db.Users.FirstOrDefaultAsync(x => x.Id == id);
            return user;
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsers()
        {
            return await db.Users.ToListAsync();
        }

        public async Task<ApplicationUser> EditUserAsync(EditUserModel editUserModel)
        {
            if (editUserModel.Id == null)
            {
                return null;
            }
            var user = await db.Users.FirstOrDefaultAsync(x => x.Id == editUserModel.Id);

            if (user == null)
            {
                return null;
            }
            db.Users.Attach(user);
            user.Email = editUserModel.Email;
            user.UserName = editUserModel.UserName;
            user.PhoneNumber = editUserModel.PhoneNumber;
            user.Country = editUserModel.Country;


            if (editUserModel.Password != user.PasswordHash)
            {
                var result = await userManager.RemovePasswordAsync(user);
                if (result.Succeeded)
                {
                    await userManager.AddPasswordAsync(user, editUserModel.Password);

                }
            }


            db.Entry(user).Property(x => x.Email).IsModified = true;
            db.Entry(user).Property(x => x.UserName).IsModified = true;
            db.Entry(user).Property(x => x.PhoneNumber).IsModified = true;
            db.Entry(user).Property(x => x.Country).IsModified = true;
            await db.SaveChangesAsync();
            return user;

        }

        public async Task<ApplicationUser> DeleteUser(string id)
        {
            

            if (id == null)
            {
                return null;
            }
            var userId = await db.Users.Where(x => x.Id == id).SingleOrDefaultAsync();
            if(userId == null)
            {
                
            }
            db.Remove(userId);
            await db.SaveChangesAsync();
            return userId;

        }

        public async Task<IEnumerable<UsersRolesModel>> GetRoleNameAsync()
        {
            var query = await (
                 from userRole in db.UserRoles
                 join users in db.Users
                 on userRole.UserId equals users.Id
                 join roles in db.Roles
                 on userRole.RoleId equals roles.Id


                 select new
                 {
                     userRole.UserId,
                     users.UserName,
                     userRole.RoleId,
                     roles.Name

                 }).ToListAsync();
            List<UsersRolesModel> usersRolesModels = new List<UsersRolesModel>();
            if (query.Count > 0)
            {
           
                for (int i=0; i<query.Count;i++)
                {
                    var model = new UsersRolesModel
                    {
                        UserId = query[i].UserId,
                        UserName = query[i].UserName,
                        RoleId = query[i].RoleId,
                        RoleName = query[i].Name
                    };
                    usersRolesModels.Add(model);
                }
               
            }
            return usersRolesModels;
          
        }

        public async Task<IEnumerable<ApplicationRole>> GetAllRoleAsync()
        {
            return await db.Roles.ToListAsync();
        }
    }
}
