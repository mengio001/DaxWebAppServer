using QuizTowerPlatform.Data.Context;
using QuizTowerPlatform.Data.DataModels;

namespace QuizTowerPlatform.Api.Services.Interfaces
{
    public interface IQuestionService
    {
        Task AddQuestion(IApiDbContext db, Question question);
    }
}
