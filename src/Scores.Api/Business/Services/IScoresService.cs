using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Scores.Api.Data.Models.Requests;
using Scores.Api.Data.Models.Responses;

namespace Scores.Api.Business.Services
{
    public interface IScoresService
    {
        Task<IList<ScoresResponse>> GetScoresGreaterThan(int number);
        Task CreateScore(ScoresRequest score);
    }
}
