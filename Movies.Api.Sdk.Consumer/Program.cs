
using System.Text.Json;
using Movies.Api.Sdk;
using Refit;

var moviesApi = RestService.For<IMoviesApi>("http://localhost:5258");

var movie = await moviesApi.GetMovieAsync("nosferatu-2024");

Console.WriteLine(JsonSerializer.Serialize(movie));


Console.ReadKey();
