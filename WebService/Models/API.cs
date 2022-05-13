using Newtonsoft.Json;

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
            public string? poster_path { get; set; }
        }

    }
}
