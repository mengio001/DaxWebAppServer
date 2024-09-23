using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizTowerPlatform.Api.Accessors;
using QuizTowerPlatform.Api.Services.Interfaces;
using QuizTowerPlatform.Model;

namespace QuizTowerPlatform.Api.Controllers
{
    public class AchievementController : BaseController
    {
        private readonly IAchievementService service;
        private readonly IMapper mapper;

        public AchievementController(IRequestAccessor request, IAchievementService service, IMapper mapper) : base(request)
        {
            this.service = service;
            this.mapper = mapper;
        }

        [HttpGet()]
        public async Task<ActionResult<IEnumerable<AllAchievementsModel>>> AllAchievements()
        {
            var model = (await this.service.GetAllAchievements(Db)).Select(mapper.Map<AllAchievementsModel>);
            return model == null ? NotFound() : Ok(model);
        }

        [HttpGet()]
        public async Task<ActionResult<IEnumerable<MyAchievementsModel>>> MyAchievements()
        {
            var model = (await this.service.GetAchievementsByUser(Db, this.User.Identity.Name))
                .Select(mapper.Map<MyAchievementsModel>);
            return model == null ? NotFound() : Ok(model);
        }
    }
}
