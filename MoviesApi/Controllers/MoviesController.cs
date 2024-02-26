using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Services;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
       
        private readonly IMoviesServices _moviesServices;
        private readonly IGenresServices _genresServices;
        private readonly IMapper _mapper;

        public MoviesController(IGenresServices genresServices, IMoviesServices moviesServices ,IMapper mapper)
        {
            _mapper = mapper;
            _genresServices = genresServices;
            _moviesServices = moviesServices;
        }

        private new List<string> _allowedExtensions =new List<string> {".jpg" ,".png"};
        private long _maxallowedPosterSize = 1048576;


        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var movies= await _moviesServices.GetAll();
            var data=_mapper.Map<IEnumerable<MoviesDetailsDto>>(movies);
            return Ok(data);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var movie =await  _moviesServices.GetById(id);
            if (movie == null)
                return NotFound($"No Movie with ID {id} !");
            var dto =_mapper.Map<MoviesDetailsDto>(movie); 
            return Ok(dto);
        }
        [HttpGet("GetByGenreId")]
        public async Task<IActionResult> GetByGenreIdAsync(byte genreId)
        {
            var movies = await _moviesServices.GetAll(genreId);
            var data = _mapper.Map<IEnumerable<MoviesDetailsDto>>(movies);
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm]MovieDto dto )
        {
            if (!_allowedExtensions.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                return BadRequest("Only .jpg and .png images are allowed!");
            if(dto.Poster.Length>_maxallowedPosterSize)
                return BadRequest("Max allowed size for poster is 1MB !");

            var isValidGenre = _genresServices.IsValidGenre(dto.GenreId);

            if(!await isValidGenre)
                return BadRequest("Invalid Genre ID!");


            using var datastream=new MemoryStream();
            await dto.Poster.CopyToAsync(datastream);
            var movies = _mapper.Map<Movie>(dto);
            movies.Poster=datastream.ToArray();
            await _moviesServices.Add (movies);
            return Ok (movies);
             
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id ,[FromForm] MovieDto dto )
        {
            var movie = await  _moviesServices.GetById (id);
            if (movie == null)
                return NotFound($"No Movie Found With ID {id} !");
            var isValidGenre = _genresServices.IsValidGenre(dto.GenreId);
            if(!await isValidGenre)
                return BadRequest("Invalid Genre ID!");
            if (dto.Poster !=null)
            {
                if (!_allowedExtensions.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                    return BadRequest("Only .jpg and .png images are allowed!");
                if (dto.Poster.Length > _maxallowedPosterSize)
                    return BadRequest("Max allowed size for poster is 1MB !");
                using var datastream = new MemoryStream();
                await dto.Poster.CopyToAsync(datastream);
                movie.Poster = datastream.ToArray();
            }
            movie.Title =dto.Title;
            movie.Year = dto.Year;
            movie.Rate = dto.Rate;
            movie.StoreLine = dto.StoreLine;
            movie.GenreId = dto.GenreId;
            _moviesServices.Update(movie);
            return Ok(movie);
        }
       [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var movie = await  _moviesServices.GetById(id);
            if (movie == null)
                return NotFound($"No Movie Found With ID {id} !");
            _moviesServices.Delete(movie);
             return Ok (movie);
        }
    }
}
