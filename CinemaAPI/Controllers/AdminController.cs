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
    }
}