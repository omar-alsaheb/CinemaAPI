using CinemaAPI.Models;
using CinemaAPI.ModelsView;
using CinemaAPI.ModelsView.Users;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaAPI.Repository.Admin
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ApplicationDb db;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        [Obsolete]
        private readonly IHostingEnvironment hosting;

        [Obsolete]
        public AdminRepository(ApplicationDb _db, UserManager<ApplicationUser> _userManager, RoleManager<ApplicationRole> _roleManager,IHostingEnvironment _hosting)
        {
            db = _db;
            userManager = _userManager;
            roleManager = _roleManager;
            hosting = _hosting;
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
            if (id == null)
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
            if (userId == null)
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

                for (int i = 0; i < query.Count; i++)
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

        //public async Task<bool> EditUserRoleAsync1(EditUserRoleModel editUserRoleModel)
        //{

        //    if (editUserRoleModel.RoleId == null || editUserRoleModel.UserId == null)
        //    {

        //        return false;
        //    }

        //    var user = db.Users.FirstOrDefaultAsync(x => x.Id == editUserRoleModel.UserId);
        //    if (user == null)
        //    {
        //        return false;
        //    }

        //    var currentRoleId = await db.UserRoles.Where(x => x.UserId == editUserRoleModel.UserId).Select(x => x.RoleId).FirstOrDefaultAsync();
        //    var currentRoleName = await db.Roles.Where(x => x.Id == currentRoleId).Select(x => x.Name).FirstOrDefaultAsync();
        //    var newRoleName = await db.Roles.Where(x => x.Id == editUserRoleModel.RoleId).Select(x => x.Name).FirstOrDefaultAsync();

        //    if (await userManager.IsInRoleAsync(user, currentRoleName))
        //    {
        //        var x = await userManager.RemoveFromRoleAsync(user, currentRoleName);
        //        if (x.Succeeded)
        //        {
        //            var s = await userManager.AddToRoleAsync(user, newRoleName);
        //            if (s.Succeeded)
        //            {
        //                return true;
        //            }
        //        }

        //        return false;
        //    }
        //}


        public async Task<bool> EditUserRoleAsync(EditUserRoleModel model)
        {
            if (model.UserId == null || model.RoleId == null)
            {
                return false;
            }

            var user = await db.Users.FirstOrDefaultAsync(x => x.Id == model.UserId);
            if (user == null)
            {
                return false;
            }

            var currentRoleId = await db.UserRoles.Where(x => x.UserId == model.UserId).Select(x => x.RoleId).FirstOrDefaultAsync();
            var currentRoleName = await db.Roles.Where(x => x.Id == currentRoleId).Select(x => x.Name).FirstOrDefaultAsync();
            var newRoleName = await db.Roles.Where(x => x.Id == model.RoleId).Select(x => x.Name).FirstOrDefaultAsync();

            if (await userManager.IsInRoleAsync(user, currentRoleName))
            {
                var x = await userManager.RemoveFromRoleAsync(user, currentRoleName);
                if (x.Succeeded)
                {
                    var s = await userManager.AddToRoleAsync(user, newRoleName);
                    if (s.Succeeded)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public async Task<IEnumerable<Category>> GetCategories()
        {
            return await db.Category.ToListAsync();
        }

        public async Task<Category> AddNewCategory(Category category)
        {
            var newCat = new Category
            {
                CategoryName = category.CategoryName
            };

            db.Category.Add(newCat);
            await db.SaveChangesAsync();
            return newCat;

        }

        public async Task<Category> UpdateCategory(Category categoryPar)
        {
            if (categoryPar == null || categoryPar.Id < 1)
            {
                return null;
            }
            var category = await db.Category.FirstOrDefaultAsync(x => x.Id == categoryPar.Id);
            if (category == null)
            {
                return null;

            }

            db.Category.Attach(category);
            category.CategoryName = categoryPar.CategoryName;
            db.Entry(category).Property(x => x.CategoryName).IsModified = true;
            await db.SaveChangesAsync();
            return category;
        }

        public async Task<bool> DeleteCategory(int id)
        {   
            
            var categoryId = await db.Category.FirstOrDefaultAsync(x => x.Id == id);
            if (categoryId==null)
            {
                return false;
            }

            db.Category.Remove(categoryId);
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<SubCategory> AddSubCategory(SubCategory model)
        {
            var subCategory = new SubCategory
            {
                SubCategoryName = model.SubCategoryName,
                CategoryId = model.CategoryId
            };
        
            db.SubCategory.Add(subCategory);
            await db.SaveChangesAsync();
            return subCategory;

        }

        public async Task<IEnumerable<SubCategory>> GetSubCategory()
        {
            var getSubCat = await db.SubCategory.Include(x => x.category).ToListAsync();
            return getSubCat;
        }

        public async Task<bool> DeleteSubCategory(int id)
        {
          
            var subId = await db.SubCategory.FirstOrDefaultAsync(x => x.Id == id);
            if (subId == null)
            {
                return false;
            }
            db.SubCategory.Remove(subId);
           await db.SaveChangesAsync();
            return true;
        }

        public async Task<SubCategory> EditSubCategory(SubCategory model)
        {

            if (model == null || model.Id < 1)
            {
                return null;
            }
            var subCat = await db.SubCategory.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (subCat == null)
            {
                return null;
            }
            db.SubCategory.Attach(subCat);
            subCat.SubCategoryName = model.SubCategoryName;
            subCat.CategoryId = model.CategoryId;

            db.Entry(subCat).Property(x => x.SubCategoryName).IsModified = true;
            db.Entry(subCat).Property(x => x.CategoryId).IsModified = true;

            await db.SaveChangesAsync();

            return subCat;


        }

        [Obsolete]
        public async Task<Actor> AddActorAsync(string actorName, IFormFile img)
        {
            var filePath = Path.Combine(hosting.WebRootPath + "/images/actors/" + img.FileName);
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                await img.CopyToAsync(fileStream);
            }
            var actor = new Actor
            {
                ActorName = actorName,
                ActorPicture = img.FileName
            };
            db.Actor.Add(actor);
            await db.SaveChangesAsync();
            return actor;
        }

        public async Task<IEnumerable<Actor>> GetActorsAsync()
        {
           return await db.Actor.ToListAsync();
        }

        public Task<Actor> GetAcortId(int id)
        {
            var actorId = db.Actor.FirstOrDefaultAsync(x => x.Id == id);
            if (actorId == null)
            {
                return null;
            }
            return actorId;
        }

        [Obsolete]
        public async Task<Actor> EditActor(int id, string actName, IFormFile img)
        {
            var actor = await db.Actor.FirstOrDefaultAsync(x => x.Id == id);

            if (actor == null)
            {
                return null;
            }

            db.Attach(actor);
            actor.ActorName = actName;
            if (img != null && img.FileName.ToLower() != actor.ActorPicture.ToLower())
            {
                var filePath = Path.Combine(hosting.WebRootPath + "/images/actors/" + img.FileName);
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await img.CopyToAsync(fileStream);
                }
                actor.ActorPicture = img.FileName;
                db.Entry(actor).Property(x => x.ActorPicture).IsModified = true;

            }
            db.Entry(actor).Property(x => x.ActorName).IsModified = true;
            await db.SaveChangesAsync();
            return actor;
            {

            }
        }

        public async Task<IList<Movie>> GetMoviesAsync()
        {
            return await db.Movie.OrderByDescending(x => x.Id).Include(x => x.subCategory).ToListAsync();
        }
    }
}
