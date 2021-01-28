using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Scores.Api.Data.Models;

namespace Scores.Api.Data
{
    public class ScoresRepository : IScoresRepository
    {
        public async Task<ScoreModel> GetScoreByPlayer(string player)
        {
            var scores = await GetScores();

            var playerScore = scores.FirstOrDefault(x => x.Player == player);

            return playerScore;
        }

        public async Task<IList<ScoreModel>> GetScores()
        {
            var text = await File.ReadAllTextAsync("Data/scores.json");

            var scores = JsonSerializer.Deserialize<IList<ScoreModel>>(text);

            return scores;
        }

        public async Task UpdateScore(ScoreModel scoreModel)
        {
            var scores = await GetScores();

            var score = scores.FirstOrDefault(x => x.Player == scoreModel.Player);
            scores.Remove(score);

            score.Score = scoreModel.Score;

            scores.Add(score);

            await SaveAsync(scores);
        }

        public async Task SaveAsync(IList<ScoreModel> scores)
        {
            var text = JsonSerializer.Serialize(scores);

            await File.WriteAllTextAsync("Data/scores.json",text);
        }
    }
}