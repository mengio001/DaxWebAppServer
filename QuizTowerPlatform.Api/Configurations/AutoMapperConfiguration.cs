using AutoMapper;
using QuizTowerPlatform.Api.Models;
using QuizTowerPlatform.Data.DataModels;
using QuizTowerPlatform.Model;

namespace QuizTowerPlatform.Api.Configurations
{
    public class AutoMapperConfiguration : Profile
    {
        public AutoMapperConfiguration()
        {
            this.CreateMap<Quiz, CreateQuizBindingModel>().ReverseMap();
            this.CreateMap<Quiz, QuizModel>().ReverseMap();
            this.CreateMap<Quiz, AllQuizzesModel>().ReverseMap();
            this.CreateMap<Quiz, SearchResultsModel>().ReverseMap();
            this.CreateMap<Question, QuestionModel>().ReverseMap();
            this.CreateMap<Question, AddQuestionBindingModel>().ReverseMap();
            this.CreateMap<UserResult, UserResultModel>().ReverseMap();


            this.CreateMap<Data.DataModels.User, UsersRanklistModel>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.TotalQuizPoints, opt => opt.MapFrom(src => src.TotalAchievementPoints.TotalQuizPoints))
                .ForMember(dest => dest.TotalAchievementPoints, opt => opt.MapFrom(src => src.TotalAchievementPoints.TotalAchievedPoints))
                .ReverseMap();

            this.CreateMap<QuizServiceModel, QuizModel>().ReverseMap();
            this.CreateMap<AnswersServiceModel, AnswerModel>().ReverseMap();
            this.CreateMap<Achievement, AddAchievementBindingModel>().ReverseMap();
            this.CreateMap<Achievement, AllAchievementsModel>().ReverseMap();
            this.CreateMap<UserAchievement, MyAchievementsModel>().ReverseMap();
        }
    }
}
