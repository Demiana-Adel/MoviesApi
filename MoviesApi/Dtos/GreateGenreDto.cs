namespace MoviesApi.Dtos
{
    public class GreateGenreDto
    {
        [MaxLength(100)]
        public string  Name { get; set; }
    }
}
