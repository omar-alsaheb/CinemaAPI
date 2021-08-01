using CinemaAPI.Models;
using CinemaAPI.ModelsView;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace CinemaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDb db;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<ApplicationRole> roleManager;

        public AccountController(ApplicationDb _db, UserManager<ApplicationUser> _userManager, SignInManager<ApplicationUser> _signInManager, RoleManager<ApplicationRole> _roleManager)
        {
            db = _db;
            userManager = _userManager;
            signInManager = _signInManager;
            roleManager = _roleManager;
        }

        [HttpPost("Register")]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            if (model == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                if (EmailExist(model.Email))
                {
                    return BadRequest("Email already exists");
                }
                var user = new ApplicationUser
                {
                    Email = model.Email,
                    PasswordHash = model.Password,
                    UserName = model.UserName
                };
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action("RegisterConfirm", "Account", new
                    { ID = user.Id, Token = HttpUtility.UrlEncode(token) }, Request.Scheme);
                    return Ok(confirmationLink);

                }

            };
            return NotFound();
        }

        private bool EmailExist(string email)
        {
            return db.Users.Any(x => x.Email == email);
        }
        [HttpGet("RegisterConfirm")]
        public async Task<IActionResult> RegisterConfirm(string ID, string Token)
        {
            if (string.IsNullOrEmpty(ID) || string.IsNullOrEmpty(Token))
            {
                return NotFound();
            }

            var user = await userManager.FindByIdAsync(ID);
            if (user == null)
            {
                return NotFound();
            }
            var result = await userManager.ConfirmEmailAsync(user, HttpUtility.UrlDecode(Token));
            var errors = result.Errors;
            if (result.Succeeded)
            {

                return Ok("Register Confirmed");
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            await CreateRole();
            await CreateAdmin();

            if (loginModel == null)
            {
                return NotFound();
            }
            var email = await userManager.FindByEmailAsync(loginModel.Email);

            if (email == null)
            {
                return NotFound();
            }
            var result = await signInManager.PasswordSignInAsync(email, loginModel.Password, loginModel.RememberMe, true);
            if (result.Succeeded)
            {
                if (await roleManager.RoleExistsAsync("User"))
                {
                    if (await userManager.IsInRoleAsync(email, "User"))
                    {
                        await userManager.AddToRoleAsync(email, "User");
                    }
                }
                var roleName = await GetRoleNameByIdUser(email.Id);
                if (roleName != null) { AddCookies(email.UserName, roleName, email.Id, loginModel.RememberMe,email.Email); }
               

                return Ok(result);
            }
            else
            {
                return BadRequest(result.IsNotAllowed);
            }
        }

        private async Task<string> GetRoleNameByIdUser(string userId)
        {
            var userRole = await db.UserRoles.FirstOrDefaultAsync(x => x.UserId == userId);
            if (userRole != null)
            {
                return await db.Roles.Where(x => x.Id == userRole.RoleId).Select(x => x.Name).FirstOrDefaultAsync();
            }
            return null;
        }

        //[Authorize(Roles ="Admin")]
        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetAllUsers()
        {
            return await db.Users.ToListAsync();
        }

        public async Task CreateAdmin()
        {
            var admin = await userManager.FindByEmailAsync("Admin");
            if (admin == null)
            {
                var newAdmin = new ApplicationUser
                {
                    Email = "admin@admin.com",
                    UserName = "Admin",
                };
                var x = await userManager.CreateAsync(newAdmin, "Omar5320261@");
                if (x.Succeeded)
                {
                    if (await roleManager.RoleExistsAsync("Admin"))
                        await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
            }
        }
        public async Task CreateRole()
        {

            if (roleManager.Roles.Count() < 1)
            {
                var role = new ApplicationRole
                {
                    Name = "User"
                };
                await roleManager.CreateAsync(role);
                role = new ApplicationRole
                {
                    Name = "Admin"
                };
                await roleManager.CreateAsync(role);
            }
        }
        public async void AddCookies(string username, string roleName, string userId, bool remember,string email)
        {

            var claim = new List<Claim> {
          new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Email, email),

          new Claim(ClaimTypes.NameIdentifier,userId),
          new Claim(ClaimTypes.Role, roleName),
            };

            var claimIdentity = new ClaimsIdentity(claim, CookieAuthenticationDefaults.AuthenticationScheme);
            if (remember)
            {
                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    IsPersistent = remember,
                    ExpiresUtc = DateTime.UtcNow.AddDays(10)
                };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimIdentity), authProperties);

            }
            else
            {
                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    IsPersistent = remember,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimIdentity), authProperties);
            }

        }

        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }

        [HttpGet("GetRoleName/{email}")]
        public async Task<String> GetRoleName(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var userRole = await db.UserRoles.FirstOrDefaultAsync(x => x.UserId == user.Id);
                if (userRole !=null)
                {
                    return await db.Roles.Where(x => x.Id == userRole.RoleId).Select(x => x.Name).FirstOrDefaultAsync();
                }
            }
            return null;
        }

        //[Authorize]
        [HttpGet("CheckUserClaims/{email}&{role}")]
        public  IActionResult CheckUserClaims(string email,string role)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userEmail != null && id != null && userRole != null)
            {
                if (email == userEmail && role == userRole)
                {
                    return StatusCode(StatusCodes.Status200OK);

                }
            }
         return StatusCode(StatusCodes.Status203NonAuthoritative);

        }

    }
};

    

