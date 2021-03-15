using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extentions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
        private readonly IUnitOfWork _unitOfWork;

        public UsersController(IUnitOfWork unitOfWork, IMapper mapper, IPhotoService photoService)
        {
            _unitOfWork = unitOfWork;
            _photoService = photoService;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery] UserParams userParams)
        {
            //logged in user
            var gender = await _unitOfWork.UserRepository.GetUserGender(User.GetUsername());

            //set the name of the current user logged
            userParams.CurrentUsername = User.GetUsername();

            //filtering the users in case of a female or male is logged in
            //userParams.Gender will always be empty
            if (string.IsNullOrEmpty(userParams.Gender))
            {
                //here will give it a value depending on who is logged in
                userParams.Gender = gender == "male" ? "female" : "male";
            }

            var users = await _unitOfWork.UserRepository.GetMembersAsync(userParams);
            Response.AddPaginationHeader(users.CurrentPage, users.PageSize,
            users.TotalCount, users.TotalPages);
            return Ok(users);
        }


        [HttpGet("{username}", Name = "GetUser")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            return await _unitOfWork.UserRepository.GetMemberAsync(username);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            //gives us the username that it is used with the token (line bellow)
            var username = User.GetUsername();
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);

            _mapper.Map(memberUpdateDto, user);

            _unitOfWork.UserRepository.Update(user);

            if (await _unitOfWork.Complete()) return NoContent();
            return BadRequest("Failed to update User");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            //get the user
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

            //get the result from the photo service
            var result = await _photoService.AddPhotoAsync(file);

            //check the result
            if (result.Error != null)
            {
                return BadRequest(result.Error.Message);
            }

            //create a new photo
            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            //check to see if the user has any photos in their collection
            if (user.Photos.Count == 0)
            {
                //if they don't have, we will set to be the main photo
                photo.IsMain = true;
            }

            //add the photo
            user.Photos.Add(photo);

            //return the photo
            if (await _unitOfWork.Complete())
            {
                //return _mapper.Map<PhotoDto>(photo);
                return CreatedAtRoute("GetUser", new { username = user.UserName }, _mapper.Map<PhotoDto>(photo));
            }

            //if something doesn't go correct, we return bad request
            return BadRequest("Problem adding photo !");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            //get the user
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

            //get the photo that we want to set it Main
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo.IsMain)
            {
                return BadRequest("This is already your main photo !");
            }

            //currrent Main Photo
            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);

            //Set Current Main Photo to false
            if (currentMain != null)
            {
                currentMain.IsMain = false;
            }

            //Set the chosen PHOTO to main
            photo.IsMain = true;

            if (await _unitOfWork.Complete())
            {
                return NoContent(); //returns 204 NoContent
            }
            return BadRequest("Failed to set Main Photo !"); //return 400
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            //get the user
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

            //to get the actual photo that we want to delete
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            //check if it is null
            if (photo == null)
            {
                return NotFound();
            }

            if (photo.IsMain)
            {
                return BadRequest("You cannot delete your main photo !");
            }

            if (photo.PublicId != null)
            {
                //deleting from cloudinary the photo
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);

                //checking if there are any errors
                if (result.Error != null)
                {
                    return BadRequest(result.Error.Message);
                }
            }
            //remove from database
            user.Photos.Remove(photo);

            if (await _unitOfWork.Complete())
            {
                return Ok();
            }

            return BadRequest("Failed to delete the photo !");
        }
    }
}