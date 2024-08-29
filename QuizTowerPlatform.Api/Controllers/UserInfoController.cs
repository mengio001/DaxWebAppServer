using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizTowerPlatform.Api.Accessors;
using QuizTowerPlatform.Api.Services.Interfaces;
using QuizTowerPlatform.Model;

namespace QuizTowerPlatform.Api.Controllers
{
    public class UserInfoController : BaseController
    {
        private readonly IUserInfoService service;
        private readonly IMapper mapper;

        public UserInfoController(IRequestAccessor request, IUserInfoService service, IMapper mapper) : base(request)
        {
            this.service = service;
            this.mapper = mapper;
        }

        [HttpGet()]
        public async Task<ActionResult<UserInfoModel>> GetUserInfo()
        {
            var model = await this.service.ReadUserInfoAsync(Db, LoggedInUser!);
            return model == null ? NotFound() : Ok(model);
        }

        [HttpGet()]
        public async Task<ActionResult<IEnumerable<UsersRanklistModel>>> RankList()
        {
            var model = (await this.service.GetUsersByTotalPoints(Db))
                .Select(mapper.Map<UsersRanklistModel>);
            return model == null ? NotFound() : Ok(model);
        }
    }
}
