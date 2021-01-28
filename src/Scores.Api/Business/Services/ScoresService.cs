using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Scores.Api.Data;
using Scores.Api.Data.Models;
using Scores.Api.Data.Models.Requests;
using Scores.Api.Data.Models.Responses;

namespace Scores.Api.Business.Services
{
    public class ScoresService : IScoresService
    {
        private readonly IMapper _mapper;
        private readonly IScoresRepository _scoresRepository;

        public ScoresService(IMapper mapper, IScoresRepository scoresRepository)
        {
            _mapper = mapper;
            _scoresRepository = scoresRepository;
        }
        
        public async Task<IList<ScoresResponse>> GetScoresGreaterThan(int number)
        {
            var all = await _scoresRepository.GetScores();

            var filtered = all.Where(x => x.Score > number).ToList();

            var scores = _mapper.Map<IList<ScoreModel>, IList<ScoresResponse>>(filtered);

            return scores;
        }

        public async Task UpdateScore(string player, ScoresRequest payload)
        {
            if (string.IsNullOrWhiteSpace(player))
            {
                throw new ArgumentNullException($"{nameof(player)} is required");
            }

            if (payload == null)
            {
                throw new ArgumentNullException($"{nameof(payload)} is required");
            }

            await _scoresRepository.UpdateScore(new ScoreModel { Player = player, Score = payload.Score });
        }
    }
}