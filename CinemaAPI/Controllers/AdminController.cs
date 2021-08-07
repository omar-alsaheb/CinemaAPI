using CinemaAPI.Models;
using CinemaAPI.ModelsView;
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

            var users= await adminRepository.GetUsers();
            if (users==null)
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

            if (ModelState.IsValid) {
                var user = await adminRepository.AddUser(userModel);
                if(user != null)
                {
                    return Ok();
                }
                
            }
            return BadRequest();
        }


    }
}
