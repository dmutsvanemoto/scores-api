using System.Collections.Generic;
using System.Threading.Tasks;
using Scores.Api.Data.Models;

namespace Scores.Api.Data
{
    public interface IScoresRepository
    {
        Task<ScoreModel> GetScoreByPlayer(string player);
        Task<IList<ScoreModel>> GetScores();
        Task UpdateScore(ScoreModel scoreModel);
    }
}
