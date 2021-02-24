using System.Linq;
using API.DTOs;
using API.Entities;
using API.Extentions;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            //we don't need to map manually one object to the other
            CreateMap<AppUser,MemberDto>()
            .ForMember(dest => dest.PhotoUrl, 
            opt => opt.MapFrom(src =>src.Photos.FirstOrDefault(x=>x.IsMain).Url))
            .ForMember(dest =>dest.Age,
            opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge())); 
            CreateMap<Photo,PhotoDto>();
            CreateMap<MemberUpdateDto,AppUser>();
            CreateMap<RegisterDto,AppUser>();
            CreateMap<Message,MessageDto>() // translates to Map from our Message(database) to MessageDto
            .ForMember(dest => dest.SenderPhotoUrl,
             opt => opt.MapFrom(src => src.Sender.Photos.FirstOrDefault(x =>x.IsMain).Url))
            .ForMember(dest => dest.RecipientPhotoUrl,
             opt => opt.MapFrom(src => src.Recipient.Photos.FirstOrDefault(x =>x.IsMain).Url)); 
        }
    }
}