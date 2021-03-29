using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;

namespace API.Interfaces
{
    public interface IPhotoManagement
    {
         Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos();
         Task<Photo>GetPhotoById(int id);
         void RemovePhoto(Photo photo);
         void UpdatePhotoForUser(Photo photo);
    }
}