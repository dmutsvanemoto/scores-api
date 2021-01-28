using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Scores.Api.Business.Services;
using Scores.Api.Data;
using Scores.Api.Data.Models.Requests;

namespace Scores.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScoresController : ControllerBase
    {
        private readonly ILogger<ScoresController> _logger;
        private readonly IScoresService _scoresService;
        private readonly IScoresRepository _scoresRepository;

        public ScoresController(ILogger<ScoresController> logger, IScoresService scoresService, IScoresRepository scoresRepository)
        {
            _logger = logger;
            _scoresService = scoresService;
            _scoresRepository = scoresRepository;
        }

        [HttpGet]
        [Route("{number}")]
        public async Task<IActionResult> GetScoresGreaterThan([FromRoute] int? number)
        {
            if (!number.HasValue)
            {
                return BadRequest("You need to specify a number!");
            }

            try
            {
                var scores = await _scoresService.GetScoresGreaterThan(number.Value);

                return Ok(scores);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Failed to {nameof(GetScoresGreaterThan)} with param {number}");
            }
            
            return StatusCode((int) HttpStatusCode.InternalServerError, "Oops something went wrong!");
        }

        [HttpPost]
        [Route("{player}")]
        public async Task<IActionResult> UpdatePlayerScore([FromRoute]string player, [FromBody] ScoresRequest payload)
        {
            if (string.IsNullOrWhiteSpace(player) || payload == null)
            {
                return BadRequest();
            }

            try
            {
                var score = await _scoresRepository.GetScoreByPlayer(player);

                if (score == null)
                {
                    return BadRequest("Player does not exist!");
                }

                await _scoresService.UpdatePlayerScore(payload);

                return NoContent();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Failed to {nameof(UpdatePlayerScore)} with payload {JsonSerializer.Serialize(payload)}");
            }

            return StatusCode((int)HttpStatusCode.InternalServerError, "Oops something went wrong!");
        }

    }
}
