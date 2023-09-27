using API.Entities;
using AutoMapper;

namespace API;

public class AutoMapperProfiles: Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<AppUser, MemberDTO>()
            .ForMember(dest => dest.PhotoUrl, opt => 
                opt.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsMain).Url))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalcualateAge()));
                //get the main photo and put into the PhotoUrl attribute
        CreateMap<Photo, PhotoDTO>();
    }
}
