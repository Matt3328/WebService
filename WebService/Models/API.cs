using Newtonsoft.Json;
using static WebService.Models.FilmModel;

namespace WebService.Models
{
    public class API
    {
        public partial class ResultsAPI
        {
            [JsonProperty("results")]
            public Movie[] results  { get; set; }
        }

        public class Movie
        {
            public int Id { get; set; }
            public string? Title { get; set; }
            public string? Overview { get; set; }

            public string? poster_path { get; set; }
            public int? Like { get; set; }
            public bool IsLiked { get; set; }
        }

    }
}
