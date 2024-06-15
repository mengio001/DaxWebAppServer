using AutoMapper;
using QuizTowerPlatform.Model;
using Image = QuizTowerPlatform.API.Entities.Image;

namespace QuizTowerPlatform.API.Profiles;

public class ImageProfile : Profile
{
    public ImageProfile()
    {
        // map from Image (entity) to Image, and back
        CreateMap<Image, Model.Image>().ReverseMap();

        // map from ImageForCreation to Image
        // Ignore properties that shouldn't be mapped
        CreateMap<ImageForCreation, Image>()
            .ForMember(m => m.FileName, options => options.Ignore())
            .ForMember(m => m.Id, options => options.Ignore())
            .ForMember(m => m.OwnerId, options => options.Ignore());

        // map from ImageForUpdate to Image
        // ignore properties that shouldn't be mapped
        CreateMap<ImageForUpdate, Image>()
            .ForMember(m => m.FileName, options => options.Ignore())
            .ForMember(m => m.Id, options => options.Ignore())
            .ForMember(m => m.OwnerId, options => options.Ignore());
    }
}