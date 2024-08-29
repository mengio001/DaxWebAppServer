using Moq;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using QuizTowerPlatform.Api.Accessors;
using QuizTowerPlatform.Api.Controllers;
using QuizTowerPlatform.Api.Services.Interfaces;
using QuizTowerPlatform.Data.DataModels;
using QuizTowerPlatform.Model;
using QuizTowerPlatform.Data.Context;

namespace QuizTowerPlatform.Api.Test
{
    [TestFixture]
    public class QuizControllerTest
    {
        private Mock<IQuizService> _mockQuizService;
        private Mock<IMapper> _mockMapper;
        private Mock<IAuthorizationService> _mockAuthorizationService;
        private Mock<IRequestAccessor> _mockRequestAccessor;
        private QuizController _controller;

        [SetUp]
        public void Setup()
        {
            _mockQuizService = new Mock<IQuizService>();
            _mockMapper = new Mock<IMapper>();
            _mockAuthorizationService = new Mock<IAuthorizationService>();
            _mockRequestAccessor = new Mock<IRequestAccessor>();

            _controller = new QuizController(_mockRequestAccessor.Object, _mockQuizService.Object, _mockMapper.Object, _mockAuthorizationService.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                // TODO: SeedData to Test CreateQuiz and CreateQuestions 
                //using var factory = new TestWebApplicationFactory<Startup>(SeedData.PopulateTestData);
                //using var client = factory.CreateClient();

            new Claim(ClaimTypes.Name, "testuser"),
                new Claim("sub", "123"),
                new Claim("given_name", "Test User"),
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        [Test]
        public async Task GetQuizzes_ShouldReturnOkWithModel()
        {
            // Arrange
            var quizEntities = new List<Quiz> { new Quiz { Id = 1, Name = "Test Quiz" } };
            var quizModels = new List<AllQuizzesModel> { new AllQuizzesModel { Id = 1, Name = "Test Quiz" } };

            _mockQuizService.Setup(s => s.AllQuizzes(It.IsAny<ApiDbContext>())).ReturnsAsync(quizEntities);
            _mockMapper.Setup(m => m.Map<AllQuizzesModel>(It.IsAny<Quiz>())).Returns(quizModels.First());

            // Act
            var actionResult = await _controller.GetQuizzes();
            var result = actionResult.Result as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EquivalentTo(quizModels));
        }

        [Test]
        public async Task CreateQuiz_ShouldReturnBadRequestWhenModelIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Name", "Required");

            // Act
            var actionResult = await _controller.CreateQuiz(new CreateQuizBindingModel());
            var result = actionResult.Result as BadRequestResult;

            // Assert
            Assert.That(result, Is.TypeOf<BadRequestResult>());
        }

        [Test]
        public async Task CreateQuiz_ShouldReturnOkWithQuizId()
        {
            // Arrange
            var model = new CreateQuizBindingModel { Name = "New Quiz" };
            var quizEntity = new Quiz { Id = 1, Name = "New Quiz" };

            _mockMapper.Setup(m => m.Map<Quiz>(model)).Returns(quizEntity);
            _mockQuizService.Setup(s => s.CreateQuiz(It.IsAny<ApiDbContext>(), It.IsAny<Quiz>())).ReturnsAsync(quizEntity.Id);

            // Act
            var actionResult = await _controller.CreateQuiz(model);
            var result = actionResult.Result as OkObjectResult;

            // Assert
            Assert.That(result, Is.TypeOf<OkObjectResult>());
            Assert.That(result.Value, Is.EqualTo(1));
        }

        [Test]
        public async Task DeleteQuiz_ShouldReturnNoContent()
        {
            // Act
            var result = await _controller.DeleteQuiz(1);

            // Assert
            Assert.That(result, Is.TypeOf<NoContentResult>());
        }

        [Test]
        public async Task GetQuiz_ShouldReturnQuizModel_WhenAuthorized()
        {
            // Arrange
            var quizEntity = new Quiz { Id = 1, Name = "Test Quiz" };
            var quizModel = new QuizModel { Id = 1, Name = "Test Quiz" };

            _mockQuizService.Setup(s => s.GetQuizById(It.IsAny<ApiDbContext>(), 1)).ReturnsAsync(quizEntity);
            _mockMapper.Setup(m => m.Map<QuizModel>(quizEntity)).Returns(quizModel);
            _mockAuthorizationService.Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), null, "UserCanEditQuiz"))
                .ReturnsAsync(AuthorizationResult.Success());

            // Act
            var result = await _controller.GetQuiz(1);

            // Assert
            Assert.That(result, Is.TypeOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.EqualTo(quizModel));
        }

        [Test]
        public async Task GetQuiz_ShouldReturnNotFound_WhenQuizDoesNotExist()
        {
            // Arrange
            _mockQuizService.Setup(s => s.GetQuizById(It.IsAny<ApiDbContext>(), 1)).ReturnsAsync((Quiz)null);

            // Act
            var result = await _controller.GetQuiz(1);

            // Assert
            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }
    }
}
