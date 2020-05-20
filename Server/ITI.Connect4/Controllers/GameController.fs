namespace ITI.Connect4.Controllers

open System
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Caching.Memory
open Microsoft.Extensions.Logging
open ITI.Connect4.Services

[<ApiController>]
[<Route("api/connect4")>]
type Connect4Controller ( logger : ILogger<Connect4Controller>,
    gameService: GameServiceDependency,
    converter : ViewModelConverterDependency,
    cache : IMemoryCache ) =

    inherit ControllerBase()

    [<HttpPost>]
    [<Route("new")>]
    member __.NewGame() =
        let { NewGame = newGame } = gameService
        match newGame cache (Guid.NewGuid()) with
        | Ok id ->
            logger.LogTrace ( sprintf "Created game %s" (id.ToString()) )
            __.Ok( id ) :> IActionResult
        | Error e ->
            logger.LogError e
            __.BadRequest( e ) :> IActionResult

    [<HttpGet>]
    [<Route("{id}")>]
    member __.GetGame (id: Guid) = 
        let { GetGame = getGame } = gameService
        let { BoardStateAsViewModel = asViewModel } = converter
        match getGame cache id with
        | Ok boardState ->
            ( boardState |> asViewModel |> __.Ok ) :> IActionResult
        | Error e -> 
            logger.LogError e;
            __.BadRequest( e ) :> IActionResult
