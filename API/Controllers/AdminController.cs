using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AdminController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        public AdminController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async  Task<ActionResult> GetUsersWithRoles()
        {
            //Includes the AppUser with the Roles, but selecting only the Id, Username and Roles
            //using this methods to include just what we want to return
            var users = await _userManager.Users
            .Include(r => r.UserRoles)
            .ThenInclude(r => r.Role)
            .OrderBy(u => u.UserName)
            .Select(u => new {
                u.Id,
                Username = u.UserName,
                Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
            })
            .ToListAsync();

            return Ok(users);
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public ActionResult GetPhotosForModeration()
        {
            return Ok("Admins or Moderatos can see this");
        }

        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult>EditRoles(string username,[FromQuery]string roles)
        {
            //pass by query params the roles
            var selectedRoles = roles.Split(",").ToArray();

            //get the user
            var user = await _userManager.FindByNameAsync(username);

            if(user==null) return NotFound("Could not find the user");

            //get the current roles
            var userRoles = await _userManager.GetRolesAsync(user);

            //we add the current roles to the user, but will not re-add the current roles his in
            var result  = await _userManager.AddToRolesAsync(user,selectedRoles.Except(userRoles));

            //check
            if(!result.Succeeded){
                return BadRequest("Failed to add to roles !");
            }

            //remove the roles that the user is currently in
            result = await _userManager.RemoveFromRolesAsync(user,userRoles.Except(selectedRoles));

            if(!result.Succeeded) return BadRequest("Failed to remove from roles !");

            return Ok(await _userManager.GetRolesAsync(user));
        }
    }
}