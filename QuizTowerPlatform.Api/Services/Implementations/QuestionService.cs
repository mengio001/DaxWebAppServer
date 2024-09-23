using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QuizTowerPlatform.Api.Services.Interfaces;
using QuizTowerPlatform.Api.Services.Security;
using QuizTowerPlatform.Data.Context;
using QuizTowerPlatform.Data.DataModels;
using QuizTowerPlatform.Model;

namespace QuizTowerPlatform.Api.Services.Implementations
{
    public class QuestionService : IQuestionService
    {
        public async Task AddQuestion(IApiDbContext db, Question question)
        {
            if (question.CorrectAnswer == question.FirstOption || question.CorrectAnswer == question.SecondOption ||
                question.CorrectAnswer == question.ThirdOption || question.CorrectAnswer == question.FourthOption)
            {
                await db.Questions.AddAsync(question);
            }
            else
            {
                return;
            }

            await db.SaveChangesAsync();
        }
    }
}
