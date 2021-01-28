using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using Scores.Api.Business.Mappers;
using Scores.Api.Business.Services;
using Scores.Api.Data;
using Scores.Api.Data.Models;
using Scores.Api.Data.Models.Requests;
using Scores.Api.Data.Models.Responses;
using Xunit;

namespace Scores.Api.Tests.Services
{
    public class ScoresServiceTests
    {
        private readonly IMapper _mapper;

        public ScoresServiceTests()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ScoreMapperProfile>();
            });

            //Assert profiles are valid
            configuration.AssertConfigurationIsValid();

            _mapper = configuration.CreateMapper();

        }


        [Fact]
        public async Task GetScoresGreaterThan_Throws_Exception()
        {
            var number = 10;

            var repository = Mock.Of<IScoresRepository>(
                x => x.GetScores() == Task.FromException<IList<ScoreModel>>(new Exception("Custom Test Exception")));

            var service = new ScoresService(_mapper, repository);
            
            Func<Task> act = async () =>
            {
                await service.GetScoresGreaterThan(number);
            };

            await act.Should()
                .ThrowAsync<Exception>()
                .WithMessage("Custom Test Exception");
        }

        [Fact]
        public async Task GetScoresGreaterThan()
        {
            var number = 10;
            var stub = new List<ScoreModel>
            {
                new ScoreModel { Player = "Dave", Score = 100 }
            };

            var repository = Mock.Of<IScoresRepository>(
                x => x.GetScores() == Task.FromResult<IList<ScoreModel>>(stub));

            var service = new ScoresService(_mapper, repository);

            var expected = _mapper.Map<IList<ScoreModel>, IList<ScoresResponse>>(stub);
            var actual = await service.GetScoresGreaterThan(number);

            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task UpdateScore_When_Player_Is_Null_Then_Throw_Exception(string player)
        {
            var repository = Mock.Of<IScoresRepository>();

            var service = new ScoresService(_mapper, repository);

            Func<Task> act = async () =>
            {
                await service.UpdateScore(player, new ScoresRequest());
            };

            await act.Should()
                .ThrowAsync<Exception>()
                .WithMessage("Value cannot be null. (Parameter 'player is required')");
        }

        [Fact]
        public async Task UpdateScore_When_Payload_Is_Null_Then_Throw_Exception()
        {
            var repository = Mock.Of<IScoresRepository>();

            var service = new ScoresService(_mapper, repository);

            Func<Task> act = async () =>
            {
                await service.UpdateScore("Dave", null);
            };

            await act.Should()
                .ThrowAsync<Exception>()
                .WithMessage("Value cannot be null. (Parameter 'payload is required')");
        }

        [Fact]
        public async Task UpdateScore_Throws_Exception()
        {
            var player = "Dave";
            var score = 100;

            var repository = Mock.Of<IScoresRepository>(
                x => x.UpdateScore(It.Is<ScoreModel>(o => o.Player == player && o.Score == score)) == Task.FromException<IList<ScoreModel>>(new Exception("Custom Test Exception")));

            var service = new ScoresService(_mapper, repository);

            Func<Task> act = async () =>
            {
                await service.UpdateScore(player, new ScoresRequest{ Score = score });
            };

            await act.Should()
                .ThrowAsync<Exception>()
                .WithMessage("Custom Test Exception");
        }

        [Fact]
        public async Task UpdateScore_Does_Not_Throw_Exception()
        {
            var player = "Dave";
            var score = 100;

            var repository = Mock.Of<IScoresRepository>(
                x => x.UpdateScore(It.Is<ScoreModel>(o => o.Player == player && o.Score == score)) == Task.CompletedTask);

            var service = new ScoresService(_mapper, repository);

            Func<Task> act = async () =>
            {
                await service.UpdateScore(player, new ScoresRequest { Score = score });
            };

            await act.Should()
                .NotThrowAsync<Exception>();
        }
    }
}
