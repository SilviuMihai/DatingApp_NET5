using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AdminController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public AdminController(UserManager<AppUser> userManager, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRoles()
        {
            //Includes the AppUser with the Roles, but selecting only the Id, Username and Roles
            //using this methods to include just what we want to return
            var users = await _userManager.Users
            .Include(r => r.UserRoles)
            .ThenInclude(r => r.Role)
            .OrderBy(u => u.UserName)
            .Select(u => new
            {
                u.Id,
                Username = u.UserName,
                Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
            })
            .ToListAsync();

            return Ok(users);
        }

        //[Authorize(Policy = "ModeratePhotoRole")]


        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles)
        {
            //pass by query params the roles
            var selectedRoles = roles.Split(",").ToArray();

            //get the user
            var user = await _userManager.FindByNameAsync(username);

            if (user == null) return NotFound("Could not find the user");

            //get the current roles
            var userRoles = await _userManager.GetRolesAsync(user);

            //we add the current roles to the user, but will not re-add the current roles his in
            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            //check
            if (!result.Succeeded)
            {
                return BadRequest("Failed to add to roles !");
            }

            //remove the roles that the user is currently in
            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if (!result.Succeeded) return BadRequest("Failed to remove from roles !");

            return Ok(await _userManager.GetRolesAsync(user));
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("photos-to-moderate")]
        public async Task<ActionResult> GetPhotosForApproval()
        {
            var photos = await _unitOfWork.PhotoRepository.GetUnapprovedPhotos();

            if (photos == null) return NoContent();

            return Ok(photos);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPut("approve-photo/{id}")]
        public async Task<ActionResult> ApprovePhoto(int id)
        {
            //get the photo that has that respective Id
            var photo = await _unitOfWork.PhotoRepository.GetPhotoById(id);

            //if it is not found, returns badrequest
            if (photo == null) return BadRequest("Photo does not exist !");

            var users = _userManager.Users.Include(p => p.Photos).AsQueryable();

            var userWithPhoto = users.FirstOrDefault(u => u.Photos.Any(p => p.AppUserId == u.Id && p.Id == photo.Id));

            if (userWithPhoto == null) return BadRequest("No user has that photo !");
            //search if it has a main photo
            var searchForMainPhoto =
            userWithPhoto.Photos.FirstOrDefault(x => x.IsMain == true);

            //if the search is null than set the photo to Approved and make it the main photo
            if (searchForMainPhoto == null)
            {
                photo.IsApproved = true;
                photo.IsMain = true;
                //update the Photo table
                _unitOfWork.PhotoRepository.UpdatePhotoForUser(photo);
            }
            else // if it has a main photo, only set to be approved
            {
                photo.IsApproved = true;
                //update the Photo table
                _unitOfWork.PhotoRepository.UpdatePhotoForUser(photo);
            }

            if (await _unitOfWork.Complete()) return Ok();

            return BadRequest("Problem approving the photo !");
        }


        [Authorize(Policy = "RequireAdminRole")]
        [HttpDelete("remove-unapproved-photo/{id}")]
        public async Task<ActionResult> RejectPhoto(int id)
        {
            var photo = await _unitOfWork.PhotoRepository.GetPhotoById(id);

            if (photo == null) return NotFound();

            _unitOfWork.PhotoRepository.RemovePhoto(photo);

            if (await _unitOfWork.Complete()) return Ok();

            return BadRequest("Problem deleting the photo !");
        }



    }
}