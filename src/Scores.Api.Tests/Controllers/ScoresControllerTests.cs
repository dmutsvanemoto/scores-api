using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Scores.Api.Business.Services;
using Scores.Api.Controllers;
using Scores.Api.Data;
using Scores.Api.Data.Models;
using Scores.Api.Data.Models.Requests;
using Scores.Api.Data.Models.Responses;
using Xunit;

namespace Scores.Api.Tests.Controllers
{
    public class ScoresControllerTests
    {
        private readonly ILogger<ScoresController> _mockLogger;

        private readonly Mock<IScoresService> _mockScoresService;
        private readonly Mock<IScoresRepository> _mockScoresRepository;

        public ScoresControllerTests()
        {
            _mockLogger = Mock.Of<ILogger<ScoresController>>();

            _mockScoresService = new Mock<IScoresService>();
            _mockScoresRepository = new Mock<IScoresRepository>();
        }

        [Fact]
        public async Task GetScoresGreaterThan_When_An_Exception_Is_Thrown_Then_Return_InternalServerError()
        {
            var number = 123;
            var expectedException = new ArgumentNullException();

            _mockScoresService
                .Setup(x => x.GetScoresGreaterThan(It.Is<int>(o => o == number)))
                .ThrowsAsync(expectedException);

            var controller = new ScoresController(_mockLogger, _mockScoresService.Object, _mockScoresRepository.Object);

            var expected = new ObjectResult("Oops something went wrong!")
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            };

            var actual = await controller.GetScoresGreaterThan(number);

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetScoresGreaterThan_When_Number_Is_NULL_Then_Return_BadRequest()
        {
            var controller = new ScoresController(_mockLogger, _mockScoresService.Object, _mockScoresRepository.Object);

            var expected = new BadRequestObjectResult("You need to specify a number!");
            var actual = await controller.GetScoresGreaterThan(null);

            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [MemberData(nameof(GetScoreGreaterThanMemberData))]
        public async Task GetScoresGreaterThan_When_Number_Is_Valid_Then_Return_Okay(int number, IList<ScoresResponse> expectedScores)
        {
            _mockScoresService
                .Setup(x => x.GetScoresGreaterThan(It.Is<int>(o => o == number)))
                .ReturnsAsync(expectedScores);

            var controller = new ScoresController(_mockLogger, _mockScoresService.Object, _mockScoresRepository.Object);

            var expectedResponse = new OkObjectResult(expectedScores);
            var actual = await controller.GetScoresGreaterThan(number);

            actual.Should().BeEquivalentTo(expectedResponse);
            (actual as OkObjectResult)?.Value.Should().BeEquivalentTo(expectedScores);

            _mockScoresService.Verify();
        }
        
        [Fact]
        public async Task CreateScore_When_An_Exception_Is_Thrown_Then_Return_InternalServerError()
        {
            var expectedException = new ArgumentNullException();

            _mockScoresRepository
                .Setup(x => x.GetScoreByPlayer(It.IsAny<string>()))
                .ReturnsAsync(new ScoreModel());

            _mockScoresService
                .Setup(x => x.UpdateScore(It.IsAny<string>(), It.IsAny<ScoresRequest>()))
                .ThrowsAsync(expectedException);

            var controller = new ScoresController(_mockLogger, _mockScoresService.Object, _mockScoresRepository.Object);

            var expected = new ObjectResult("Oops something went wrong!")
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            };

            var actual = await controller.UpdatePlayerScore("Dave", new ScoresRequest());

            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [MemberData(nameof(UpdatePlayerScoreData))]
        public async Task CreateScore_When_Request_Is_Invalid_Then_Return_BadRequest(string player, ScoresRequest payload)
        {
            var controller = new ScoresController(_mockLogger, _mockScoresService.Object, _mockScoresRepository.Object);

            var expected = new BadRequestResult();
            var actual = await controller.UpdatePlayerScore(player, payload);

            actual.Should().BeEquivalentTo(expected);
        }

        public static TheoryData<string, ScoresRequest> UpdatePlayerScoreData => new TheoryData<string, ScoresRequest>
        {
            { string.Empty, new ScoresRequest() },
            { null, new ScoresRequest() },
            { string.Empty, null },
            { null, null },
            { "Dave", new ScoresRequest() },
            { "Dave", null },
            { "Dave", new ScoresRequest() { Score = -1 } },
            { "Dave", new ScoresRequest() { Score = 1239 } },
        };

        [Fact]
        public async Task CreateScore_When_Player_Does_Not_Exist_Then_Return_BadRequest()
        {
            _mockScoresRepository
                .Setup(x => x.GetScoreByPlayer(It.IsAny<string>()))
                .ReturnsAsync(null as ScoreModel);

            var controller = new ScoresController(_mockLogger, _mockScoresService.Object, _mockScoresRepository.Object);

            var expected = new BadRequestObjectResult("Player does not exist!");
            var actual = await controller.UpdatePlayerScore("Unknown", new ScoresRequest());

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task CreateScore_When_Payload_Is_Valid_Then_Return_NoContent()
        {
            _mockScoresRepository
                .Setup(x => x.GetScoreByPlayer(It.IsAny<string>()))
                .ReturnsAsync(new ScoreModel());

            _mockScoresService
                .Setup(x => x.UpdateScore(It.Is<string>(o => o == "Dave"), It.IsAny<ScoresRequest>()))
                .Returns(Task.CompletedTask);

            var controller = new ScoresController(_mockLogger, _mockScoresService.Object, _mockScoresRepository.Object);

            var expectedResponse = new NoContentResult();
            var actual = await controller.UpdatePlayerScore("Dave", new ScoresRequest{ Score = 123 });

            actual.Should().BeEquivalentTo(expectedResponse);

            _mockScoresService.Verify();
        }

        public static IList<ScoresResponse> ScoreResponses => new List<ScoresResponse>
        {
            new ScoresResponse() {Player = "David", Score = 1},
            new ScoresResponse() {Player = "Liam", Score = 19},
            new ScoresResponse() {Player = "Amy", Score = 222},
            new ScoresResponse() {Player = "Alex", Score = 9999}
        };

        public static TheoryData<int, IList<ScoresResponse>> GetScoreGreaterThanMemberData
        {
            get
            {
                var data = new TheoryData<int, IList<ScoresResponse>>();

                data.Add(0, ScoreResponses.Where(x => x.Score > 0).ToList());
                data.Add(123, ScoreResponses.Where(x => x.Score > 123).ToList());
                data.Add(6789, ScoreResponses.Where(x => x.Score > 6789).ToList());

                return data;
            }
        }
    }
}
