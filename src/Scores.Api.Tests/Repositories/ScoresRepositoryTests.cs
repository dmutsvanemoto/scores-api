using System.Threading.Tasks;
using FluentAssertions;
using Scores.Api.Data;
using Scores.Api.Data.Models;
using Xunit;

namespace Scores.Api.Tests.Repositories
{
    public class ScoresRepositoryTests
    {
        [Fact]
        public async Task GetScoreByPlayer_When_Player_Is_Unknown_Then_Return_Null()
        {
            var player = "Unknown";

            var repo = new ScoresRepository();

            var score = await repo.GetScoreByPlayer(player);

            score.Should().BeNull();
        }

        [Fact]
        public async Task GetScoreByPlayer_When_Player_Exists_Then_Return_Score()
        {
            var player = "Jonny";

            var repo = new ScoresRepository();
            
            var score = await repo.GetScoreByPlayer(player);

            score.Should().NotBeNull();
            score.Score.Should().Be(5);
            score.Player.Should().Be(player);
        }


        [Fact]
        public async Task GetScores_Returns_Scores_From_Json()
        {
            var repo = new ScoresRepository();

            var scores = await repo.GetScores();

            scores.Should().NotBeEmpty();
        }

        [Fact]
        public async Task UpdateScore_Saves_To_Json()
        {
            var player = "Jonny";
            var score = 9000;

            var repo = new ScoresRepository();

            await repo.UpdateScore(new ScoreModel { Player = player, Score = score });

            var scores = await repo.GetScores();

            scores.Should().NotBeEmpty();
            scores.Should().Contain(x => x.Player == player && x.Score == score);
        }
    }
}
