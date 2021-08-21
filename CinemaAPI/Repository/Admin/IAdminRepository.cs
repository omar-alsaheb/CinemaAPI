using CinemaAPI.Models;
using CinemaAPI.ModelsView;
using CinemaAPI.ModelsView.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
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

        public Task<bool> EditUserRoleAsync(EditUserRoleModel editUserRoleModel);

        public Task<IEnumerable<Category>> GetCategories();

        public Task<Category> AddNewCategory(Category category);
        public Task<Category> UpdateCategory(Category category);

        public Task<bool> DeleteCategory(int id);

        public Task<SubCategory> AddSubCategory(SubCategory subCategory);
        public Task<IEnumerable<SubCategory>> GetSubCategory();
        public Task<bool> DeleteSubCategory(int id);
        public Task<SubCategory> EditSubCategory(SubCategory subCategory);
        Task<Actor> AddActorAsync(string actorName, IFormFile img);
        Task<IEnumerable<Actor>> GetActorsAsync();
        public Task<Actor> GetAcortId(int id);
        Task<Actor> EditActor(int id, string actName, IFormFile img);
        Task<IList<Movie>> GetMoviesAsync();
    }
}
