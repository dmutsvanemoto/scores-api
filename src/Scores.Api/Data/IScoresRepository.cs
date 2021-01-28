using System.Threading.Tasks;
using Scores.Api.Data.Models;

namespace Scores.Api.Data
{
    public interface IScoresRepository
    {
        Task<ScoreModel> GetScoreByPlayer(string player);
    }
}
