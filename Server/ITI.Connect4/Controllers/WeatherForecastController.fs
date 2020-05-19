namespace ITI.Connect4.Controllers

open System
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open ITI.Connect4

[<ApiController>]
[<Route("api/[controller]")>]
type WeatherForecastController (logger : ILogger<WeatherForecastController>) =
    inherit ControllerBase()

    let summaries = [| "Freezing"; "Bracing"; "Chilly"; "Cool"; "Mild"; "Warm"; "Balmy"; "Hot"; "Sweltering"; "Scorching" |]

    [<HttpGet>]
    member __.GetWeatherForecast() : WeatherForecast[] =
        let rng = System.Random()
        [|
            for index in 0..4 ->
                { Date = DateTime.Now.AddDays(float index)
                  TemperatureC = rng.Next(-20,55)
                  Summary = summaries.[rng.Next(summaries.Length)] }
        |]
