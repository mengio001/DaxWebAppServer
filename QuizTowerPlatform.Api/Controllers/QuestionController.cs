using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizTowerPlatform.Api.Accessors;
using QuizTowerPlatform.Api.Services.Interfaces;
using QuizTowerPlatform.Data.DataModels;
using QuizTowerPlatform.Model;

namespace QuizTowerPlatform.Api.Controllers
{
    public class QuestionController : BaseController
    {
        private readonly IQuestionService service;
        private readonly IMapper mapper;

        public QuestionController(IRequestAccessor request, IQuestionService questionService, IMapper mapper) : base(request)
        {
            this.service = questionService;
            this.mapper = mapper;
        }

        [HttpPost()]
        [Authorize(Policy = "UserCanAddQuiz")]
        [Authorize(Policy = "ClientApplicationCanWrite")]
        public async Task<ActionResult<int>> AddQuestion([FromBody] AddQuestionBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var question = mapper.Map<Question>(model);

            await this.service.AddQuestion(Db, question);

            return Ok(model.QuizId);
        }
    }
}
