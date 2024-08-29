using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using QuizTowerPlatform.Api.Accessors;
using QuizTowerPlatform.Api.Models;
using QuizTowerPlatform.Api.Services.Interfaces;
using QuizTowerPlatform.Api.Services.Security;
using QuizTowerPlatform.Data.DataModels;
using QuizTowerPlatform.Model;

namespace QuizTowerPlatform.Api.Controllers
{
    public class QuizController : BaseController
    {
        private readonly IQuizService service;
        private readonly IMapper mapper;
        private readonly IAuthorizationService authorizationService;

        public QuizController(IRequestAccessor request, IQuizService service, IMapper mapper, IAuthorizationService authorizationService) : base(request)
        {
            this.service = service;
            this.mapper = mapper;
            this.authorizationService = authorizationService;
        }


        [HttpGet()]
        public async Task<ActionResult<IEnumerable<AllQuizzesModel>>> GetQuizzes()
        {
            var model = (await this.service.AllQuizzes(Db))
                .Select(mapper.Map<AllQuizzesModel>);
            return model == null ? NotFound() : Ok(model);
        }

        [HttpPost()]
        [Authorize(Policy = "UserCanAddQuiz")]
        [Authorize(Policy = "ClientApplicationCanWrite")]
        public async Task<ActionResult<int>> CreateQuiz([FromBody] CreateQuizBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var quiz = this.mapper.Map<Quiz>(model);

            var id = await this.service.CreateQuiz(Db, quiz);

            return Ok(id);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "UserCanEditQuiz")]
        [Authorize(Policy = "ClientApplicationCanWrite")]
        public async Task<IActionResult> DeleteQuiz(int id)
        {
            await this.service.DeleteQuiz(Db, id);

            return NoContent();
        }

        [HttpGet("{id}", Name = "GetQuizById")]
        [Authorize(Policy = "ClientApplicationCanRead")]
        public async Task<IActionResult> GetQuiz(int id)
        {
            var getQuiz = await this.service.GetQuizById(Db, id);

            if (getQuiz == null)
            {
                return NotFound();
            }

            var model = mapper.Map<QuizModel>(getQuiz);
            var userCanEditQuiz = await authorizationService.AuthorizeAsync(User, null, "UserCanEditQuiz");
            if (!userCanEditQuiz.Succeeded)
            {
                foreach (var question in model.QuizQuestions)
                {
                    question.CorrectAnswer = null;
                }
                if (model.Answers != null && model.Answers.Any())
                {
                    model.Answers.Clear();
                }
            }
            return model == null ? NotFound() : Ok(model);
        }

        [HttpPost()]
        public async Task<ActionResult<int>> StartQuiz(int id, [FromBody] StartQuizModel model)
        {
            var interactiveUserName = User?.Identity?.Name ??
                                      HttpContext.User.Claims.FirstOrDefault(c => c.Type == "given_name")?.Value;

            if (string.IsNullOrEmpty(interactiveUserName))
            {
                interactiveUserName = await GetUsernameFromTokenAsync();
            }

            await service.StartQuiz(Db, model, interactiveUserName);

            return Ok(model.QuizId);
        }

        private async Task<string> GetUsernameFromTokenAsync()
        {
            var accessibleUserName = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

            if (string.IsNullOrEmpty(accessibleUserName))
            {
                throw new Exception("User identifier is missing from token.");
            }

            return await Db.AspNetUser
                .Where(uf => uf.SubjectId.ToString() == accessibleUserName)
                .Select(uf => uf.UserName)
                .SingleOrDefaultAsync() ?? "";
        }

        // TODO: GetSearchingResults();
    }
}
