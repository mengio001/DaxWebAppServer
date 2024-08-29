using QuizTowerPlatform.Api.Models;
using QuizTowerPlatform.Data.Context;
using QuizTowerPlatform.Data.DataModels;
using QuizTowerPlatform.Model;

namespace QuizTowerPlatform.Api.Services.Interfaces
{
    public interface IQuizService
    {
        Task<int> CreateQuiz(IApiDbContext db, Quiz quiz);

        Task DeleteQuiz(IApiDbContext db, int id);

        Task<IEnumerable<Quiz>> AllQuizzes(IApiDbContext db);

        Task<Quiz> GetQuizById(IApiDbContext db, int id);

        Task StartQuiz(IApiDbContext db, StartQuizModel model, string username);

        Task<List<Quiz>> GetSearchingResults(IApiDbContext db, string searchTerm);
    }
}
