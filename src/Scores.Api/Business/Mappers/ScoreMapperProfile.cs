using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Scores.Api.Data.Models;
using Scores.Api.Data.Models.Responses;

namespace Scores.Api.Business.Mappers
{
    public class ScoreMapperProfile : Profile
    {
        public ScoreMapperProfile()
        {
            CreateMap<ScoreModel, ScoresResponse>()
                .ForMember(destinationMember => destinationMember.Player,
                    memberOptions => memberOptions.MapFrom(sourceMember => sourceMember.Player))
                .ForMember(destinationMember => destinationMember.Score,
                    memberOptions => memberOptions.MapFrom(sourceMember => sourceMember.Score));
        }
    }
}
