﻿using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Movies.Api.Sdk;
using Movies.Api.Sdk.Consumer;
using Movies.Contracts.Requests;
using Refit;


var services = new ServiceCollection();

services.AddHttpClient();
services.AddSingleton<AuthTokenProvider>();

services.AddRefitClient<IMoviesApi>(s => new RefitSettings
{
    AuthorizationHeaderValueGetter = async (m, token) => await s.GetRequiredService<AuthTokenProvider>().GetTokenAsync()
})
    .ConfigureHttpClient(x => x.BaseAddress = new Uri("http://localhost:5258"));

var provider = services.BuildServiceProvider();
var moviesApi = provider.GetRequiredService<IMoviesApi>();

//var movie = await moviesApi.GetMovieAsync("nosferatu-2024");
//Console.WriteLine(JsonSerializer.Serialize(movie));

var movies = await moviesApi.GetMoviesAsync(new GetAllMoviesRequest());

foreach (var movie in movies.Items)
{
    Console.WriteLine(JsonSerializer.Serialize(movie));
}


Console.ReadKey();
