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
using Scores.Api.Data.Models.Requests;

namespace Scores.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScoresController : ControllerBase
    {
        private readonly ILogger<ScoresController> _logger;
        private readonly IScoresService _scoresService;

        public ScoresController(ILogger<ScoresController> logger, IScoresService scoresService)
        {
            _logger = logger;
            _scoresService = scoresService;
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
        [Route("")]
        public async Task<IActionResult> CreateScore([FromBody] ScoresRequest payload)
        {
            if (payload == null)
            {
                return BadRequest("Unrecognized payload");
            }

            try
            {
                await _scoresService.CreateScore(payload);

                return NoContent();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Failed to {nameof(CreateScore)} with payload {JsonSerializer.Serialize(payload)}");
            }

            return StatusCode((int)HttpStatusCode.InternalServerError, "Oops something went wrong!");
        }

    }
}
