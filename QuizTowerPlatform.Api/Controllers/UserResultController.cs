using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using QuizTowerPlatform.Api.Accessors;
using QuizTowerPlatform.Api.Services.Interfaces;
using QuizTowerPlatform.Data.DataModels;
using QuizTowerPlatform.Model;

namespace QuizTowerPlatform.Api.Controllers
{
    public class UserResultController : BaseController
    {
        private readonly IUserResultService service;
        private readonly IMapper mapper;

        public UserResultController(IRequestAccessor request, IUserResultService service, IMapper mapper) : base(request)
        {
            this.service = service;
            this.mapper = mapper;
        }

        [HttpGet("{id}", Name = "GetUserResultById")]
        public async Task<IActionResult> Result(int id)
        {
            var getUserResult = await this.service.GetUserResultById(Db, id, this.User.Identity.Name);

            var userResult = mapper.Map<UserResultModel>(getUserResult);

            try
            {
                if (userResult?.User == null)
                {
                    return BadRequest();
                }
            }
            catch (Exception)
            {

                return NotFound();
            }

            return Ok(userResult);
        }

        [HttpGet("{username}", Name = "GetAllUserResultsByUser")]
        public async Task<IActionResult> MyResults(string username)
        {
            try
            {
                var userResults = (await this.service.GetAllUserResultsByUser(Db, this.User.Identity.Name ?? username))
                    .Select(mapper.Map<UserResultModel>);

                return Ok(userResults);
            }
            catch (Exception)
            {

                return NotFound();
            }
        }
    }
}
