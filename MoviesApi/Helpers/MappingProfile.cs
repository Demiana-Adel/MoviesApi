﻿using AutoMapper;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace MoviesApi.Helpers
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<Movie, MoviesDetailsDto>();
            CreateMap<MovieDto , Movie>()
                .ForMember(src => src.Poster , opt => opt.Ignore());
        }
    }
}
