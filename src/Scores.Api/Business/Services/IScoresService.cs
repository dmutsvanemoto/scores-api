using System.Collections.Generic;
using System.Threading.Tasks;
using Scores.Api.Data.Models.Requests;
using Scores.Api.Data.Models.Responses;

namespace Scores.Api.Business.Services
{
    public interface IScoresService
    {
        Task<IList<ScoresResponse>> GetScoresGreaterThan(int number);
        Task UpdateScore(string player, ScoresRequest payload);
    }
}
