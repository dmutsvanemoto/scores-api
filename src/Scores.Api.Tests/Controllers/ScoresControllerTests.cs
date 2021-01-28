using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Scores.Api.Business.Services;
using Scores.Api.Controllers;
using Scores.Api.Data.Models.Requests;
using Scores.Api.Data.Models.Responses;
using Xunit;

namespace Scores.Api.Tests.Controllers
{
    public class ScoresControllerTests
    {
        [Fact]
        public async Task GetScoresGreaterThan_When_An_Exception_Is_Thrown_Then_Return_InternalServerError()
        {
            var number = 123;
            var mockScoresService = new Mock<IScoresService>();
            var mockLogger = Mock.Of<ILogger<ScoresController>>();

            var expectedException = new ArgumentNullException();

            mockScoresService
                .Setup(x => x.GetScoresGreaterThan(It.Is<int>(o => o == number)))
                .ThrowsAsync(expectedException);

            var controller = new ScoresController(mockLogger, mockScoresService.Object);

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
            var mockScoresService = new Mock<IScoresService>();
            var mockLogger = Mock.Of<ILogger<ScoresController>>();

            var controller = new ScoresController(mockLogger, mockScoresService.Object);

            var expected = new BadRequestObjectResult("You need to specify a number!");
            var actual = await controller.GetScoresGreaterThan(null);

            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [MemberData(nameof(GetScoreGreaterThanMemberData))]
        public async Task GetScoresGreaterThan_When_Number_Is_Valid_Then_Return_Okay(int number, IList<ScoresResponse> expectedScores)
        {
            var mockScoresService = new Mock<IScoresService>();
            var mockLogger = Mock.Of<ILogger<ScoresController>>();

            mockScoresService
                .Setup(x => x.GetScoresGreaterThan(It.Is<int>(o => o == number)))
                .ReturnsAsync(expectedScores);

            var controller = new ScoresController(mockLogger, mockScoresService.Object);

            var expectedResponse = new OkObjectResult(expectedScores);
            var actual = await controller.GetScoresGreaterThan(number);

            actual.Should().BeEquivalentTo(expectedResponse);
            (actual as OkObjectResult)?.Value.Should().BeEquivalentTo(expectedScores);

            mockScoresService.Verify();
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


        [Fact]
        public async Task CreateScore_When_An_Exception_Is_Thrown_Then_Return_InternalServerError()
        {
            var mockScoresService = new Mock<IScoresService>();
            var mockLogger = Mock.Of<ILogger<ScoresController>>();

            var expectedException = new ArgumentNullException();

            mockScoresService
                .Setup(x => x.CreateScore(It.IsAny<ScoresRequest>()))
                .ThrowsAsync(expectedException);

            var controller = new ScoresController(mockLogger, mockScoresService.Object);

            var expected = new ObjectResult("Oops something went wrong!")
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            };

            var actual = await controller.CreateScore(new ScoresRequest());

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task CreateScore_When_Number_Is_NULL_Then_Return_BadRequest()
        {
            var mockScoresService = new Mock<IScoresService>();
            var mockLogger = Mock.Of<ILogger<ScoresController>>();

            var controller = new ScoresController(mockLogger, mockScoresService.Object);

            var expected = new BadRequestObjectResult("Unrecognized payload");
            var actual = await controller.CreateScore(null);

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task CreateScore_When_Payload_Is_Valid_Then_Return_NoContent()
        {
            var mockScoresService = new Mock<IScoresService>();
            var mockLogger = Mock.Of<ILogger<ScoresController>>();

            mockScoresService
                .Setup(x => x.CreateScore(It.IsAny<ScoresRequest>()))
                .Returns(Task.CompletedTask);

            var controller = new ScoresController(mockLogger, mockScoresService.Object);

            var expectedResponse = new NoContentResult();
            var actual = await controller.CreateScore(new ScoresRequest{ Player = "Dave", Score = 123 });

            actual.Should().BeEquivalentTo(expectedResponse);

            mockScoresService.Verify();
        }
    }
}
