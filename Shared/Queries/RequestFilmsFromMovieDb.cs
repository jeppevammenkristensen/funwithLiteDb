using MediatR;
using System;
using System.Net;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Shared.Configuration;

namespace Shared.Queries
{
    public class RequestFilmsFromMovieDb : IRequest<MovieDbSearchMovieResponse>

    {
        public string Query { get; set; }
    }

    public class RequestFilmsFroMovieDbRequestHandler : IRequestHandler<RequestFilmsFromMovieDb, MovieDbSearchMovieResponse>
    {
        private IConfigurationRoot _configurationRoot;

        public RequestFilmsFroMovieDbRequestHandler(IConfigurationRoot configurationRoot)
        {
            _configurationRoot = configurationRoot;
        }

        public async Task<MovieDbSearchMovieResponse> Handle(RequestFilmsFromMovieDb request, CancellationToken cancellationToken)
        {
            var api = _configurationRoot[ConfigurationKeys.MovieDbApiKey];
               
            
            var client = new HttpClient();
            var result = await client.GetAsync(
                $"https://api.themoviedb.org/3/search/movie?include_adult=false&page=1&pageSize=5&query={request.Query}&language=da-DK&api_key={api}");

            var content = await result.Content.ReadAsStringAsync();
            return MovieDbSearchMovieResponse.FromJson(content);
        }
    }




    public partial class MovieDbSearchMovieResponse
    {
        [JsonProperty("page")] public long Page { get; set; }

        [JsonProperty("total_results")] public long TotalResults { get; set; }

        [JsonProperty("total_pages")] public long TotalPages { get; set; }

        [JsonProperty("results")] public Result[] Results { get; set; }
    }

    public partial class Result
    {
        [JsonProperty("vote_count")] public long VoteCount { get; set; }

        [JsonProperty("id")] public long Id { get; set; }

        [JsonProperty("video")] public bool Video { get; set; }

        [JsonProperty("vote_average")] public double VoteAverage { get; set; }

        [JsonProperty("title")] public string Title { get; set; }

        [JsonProperty("popularity")] public double Popularity { get; set; }

        [JsonProperty("poster_path")] public string PosterPath { get; set; }

        [JsonProperty("original_language")] public string OriginalLanguage { get; set; }

        [JsonProperty("original_title")] public string OriginalTitle { get; set; }

        [JsonProperty("genre_ids")] public long[] GenreIds { get; set; }

        [JsonProperty("backdrop_path")] public string BackdropPath { get; set; }

        [JsonProperty("adult")] public bool Adult { get; set; }

        [JsonProperty("overview")] public string Overview { get; set; }

        [JsonProperty("release_date")] public DateTime? ReleaseDate { get; set; }
    }

    public partial class MovieDbSearchMovieResponse
    {
        public static MovieDbSearchMovieResponse FromJson(string json) =>
            JsonConvert.DeserializeObject<MovieDbSearchMovieResponse>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this MovieDbSearchMovieResponse self) =>
            JsonConvert.SerializeObject(self, Converter.Settings);
    }

    public class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
        };
    }
}

