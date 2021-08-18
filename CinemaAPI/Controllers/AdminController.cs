using CinemaAPI.Models;
using CinemaAPI.ModelsView;
using CinemaAPI.ModelsView.Users;
using CinemaAPI.Repository.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AdminController : ControllerBase
    {
        private readonly IAdminRepository adminRepository;

        public AdminController(IAdminRepository _adminRepository)
        {
            adminRepository = _adminRepository;
        }
        //[Authorize(Roles = "Admin")]
        [HttpGet("GetAllUsers")]
        public async Task<IEnumerable<ApplicationUser>> GetAllUsers()
        {

            var users = await adminRepository.GetUsers();
            if (users == null)
            {
                return null;
            }
            else
            {
                return users;
            }
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost("AddUser")]
        public async Task<IActionResult> AddUser(AddUserModel userModel)
        {

            if (ModelState.IsValid)
            {
                var user = await adminRepository.AddUser(userModel);
                if (user != null)
                {
                    return Ok();
                }

            }
            return BadRequest();
        }

        [HttpGet("GetUser/{id}")]
        public async Task<ActionResult<ApplicationUser>> GetUser(string id)
        {

            if (id == null)
            {
                return NotFound();

            }
            var user = await adminRepository.GetUser(id);
            if (user != null)
            {
                return user;

            }
            return BadRequest();

        }

        [HttpPut("EditUser")]
        public async Task<ActionResult<ApplicationUser>> EditUser(EditUserModel editUserModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var res = await adminRepository.EditUserAsync(editUserModel);
            if (res != null)
            {

                return res;

            }

            return BadRequest();
        }

        [HttpDelete("DeleteUser/{id}")]

        public async Task<ActionResult<ApplicationUser>> DeleteUser(string id)
        {

            if (id == null)
            {
                return BadRequest();

            }

            var userId = await adminRepository.DeleteUser(id);
            return Ok();

        }


        [HttpGet("GetRoleName")]
        public async Task<IEnumerable<UsersRolesModel>> GetRoleName()
        {
            var userRole = await adminRepository.GetRoleNameAsync();

            return userRole;
        }


        [HttpGet("GetUserRole")]
        public async Task<IEnumerable<ApplicationRole>> GetUserRole()
        {

            var roleName = await adminRepository.GetAllRoleAsync();
            if (roleName == null)
            {
                return null;
            }
            
            return roleName;
            
        }

        [Route("EditUserRole")]
        [HttpPut]
        public async Task<IActionResult> EditUserRole(EditUserRoleModel model)
        {
            if (ModelState.IsValid)
            {
                var x = await adminRepository.EditUserRoleAsync(model);
                if (x)
                {
                    return Ok();
                }
            }
            return BadRequest();
        }

        [HttpGet("GetAllCategory")]
        public async Task<IEnumerable<Category>> GetAllCategory()
        {

            var cate = await adminRepository.GetCategories();

            return cate;


        }

        [HttpPost("AddNewCategory")]
        public async Task<IActionResult> AddNewCategory(Category category)
        {

            if (ModelState.IsValid)
            {
                var cat = await adminRepository.AddNewCategory(category);
                if (cat != null)
                {
                    return Ok(cat);
                }

            }
            return BadRequest();
        }
        [HttpPut("UpdateCategory")]
        public async Task<IActionResult> UpdateCategory(Category category)
        {

            if (ModelState.IsValid)
            {
                var cat = await adminRepository.UpdateCategory(category);
                if (cat != null)
                {
                    return Ok(cat);
                }

            }
            return BadRequest();
        }

        [HttpDelete("DeleteCategory/{id}")]

        public async Task<ActionResult<ApplicationUser>> DeleteCategory(int id)
        {

            if (id == null)
            {
                return BadRequest();

            }

            var userId = await adminRepository.DeleteCategory(id);
            if (userId == false)
            {
                return BadRequest();
            }
            return Ok();

        }

        [HttpPost("AddSubCategory")]
        public async Task<IActionResult> AddSubCategory(SubCategory model)
        {
            if (model == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {

                var subCat = await adminRepository.AddSubCategory(model);
                if (subCat != null)
                {
                    return Ok(subCat);
                }
               
            }
            return BadRequest();

        }

        [HttpGet("GetSubCategory")]
        public async Task<IEnumerable<SubCategory>> GetSubCategory()
        {

            return await adminRepository.GetSubCategory();
        }

        [HttpDelete("DeleteSubCategory/{id}")]
        public async Task<ActionResult<SubCategory>> DeleteSubCategory(int id)
        {
            if (id==null)
            {
                return BadRequest();
            }

           var subCatId= await adminRepository.DeleteSubCategory(id);
            if (subCatId == false)
            {
                return NotFound();
            }
            return Ok();

        }

        [HttpPut("EditSubCategory")]
        public async Task<IActionResult> EditSubCategory(SubCategory model)
        {
            if (model == null)
            {
                return BadRequest();

            }
            if (ModelState.IsValid)
            {
                var editSub = await adminRepository.EditSubCategory(model);
                return Ok(editSub);
            }
            return Ok();

        }

        [HttpPost("AddActor")]
        public async Task<IActionResult> AddActor()
        {
            var actorName = HttpContext.Request.Form["actorName"];
            var img = HttpContext.Request.Form.Files["image"];
            if (!string.IsNullOrEmpty(actorName) && img !=null && img.Length>0)
            {
                var actor = await adminRepository.AddActorAsync(actorName, img);
                if (actor != null)
                {
                    return Ok();
                }
                return BadRequest();
            }
            return BadRequest();

        }


        [HttpGet("GetActors")]
        public async Task<IEnumerable<Actor>> GetActors()
        {

            var actors = await adminRepository.GetActorsAsync();
            if (actors == null)
            {
                return null;
            }
            else
            {
                return actors;
            }
        }


    }

    
   

  

}