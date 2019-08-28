using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnboardingApp.Models;

namespace OnboardingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private UserManager<ApplicationUser> _userManager;
        public UserProfileController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize]
        //GET : /api/UserProfile
        public async Task<Object> GetUserProfile()
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);
            return new{
                user.FullName,
                user.Email,
                user.UserName
            };
        }

        [HttpGet]
        [Route("ListUsers")]
        public IActionResult GetListUsers()
        {
            List<ApplicationUserModel> users = new List<ApplicationUserModel>();
            foreach (var user in _userManager.Users)
            {
                users.Add(new ApplicationUserModel{
                    FullName=user.FullName,
                    Email=user.Email,
                    UserName=user.UserName
                });

            }
            return Ok(users);
        }

        [HttpGet]
        [Route("GetUserByID/{id}")]
        public async Task<IActionResult> GetUserByID(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user != null)
            {
                return Ok(new{
                    user.FullName,
                    user.Email,
                    user.UserName
                });
            }

            return NotFound();
        }

        [HttpGet]
        [Route("GetUserByName/{username}")]
        public async Task<IActionResult> GetUserByName(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user != null)
            {
                return Ok(new
                {
                    user.FullName,
                    user.Email,
                    user.UserName
                });
            }

            return NotFound();

        }


        [HttpGet]
        [Route("EditUser/{id}")]
        public async Task<IActionResult> UpdateUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            ApplicationUserModel updatedUser;

            if (user != null)
            {
               updatedUser =  new ApplicationUserModel
                {
                    ID = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    UserName = user.UserName
                };

                return Ok(updatedUser);
            }

            return NotFound();
        }

        [HttpPost]
        [Route("EditUser")]
        public async Task<IActionResult> UpdateUser(ApplicationUserModel model)
        {
            var user = await _userManager.FindByIdAsync(model.ID);
           
            if (user != null)
            {
                user.FullName = model.FullName;
                user.Email = model.Email;
                user.UserName = model.UserName;
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return Ok(user);
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    return BadRequest(ModelState); 
                    

                } 
                
            }

            return NotFound();
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("ForAdmin")]
        public string GetForAdmin()
        {
            return "Web method for Admin";
        }

        [HttpGet]
        [Authorize(Roles = "Customer")]
        [Route("ForCustomer")]
        public string GetCustomer()
        {
            return "Web method for Customer";
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Customer")]
        [Route("ForAdminOrCustomer")]
        public string GetForAdminOrCustomer()
        {
            return "Web method for Admin or Customer";
        }
    }
}
