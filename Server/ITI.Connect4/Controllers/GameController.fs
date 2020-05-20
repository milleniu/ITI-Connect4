namespace ITI.Connect4.Controllers

open System
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Caching.Memory
open Microsoft.Extensions.Logging
open ITI.Connect4.Models
open ITI.Connect4.Services

[<ApiController>]
[<Route("api/connect4")>]
type Connect4Controller ( logger : ILogger<Connect4Controller>,
    gameManager : GameManagerDependency,
    persistence: PersistenceServiceDependency,
    converter : ViewModelConverterDependency,
    cache : IMemoryCache ) =

    inherit ControllerBase()

    [<HttpPost>]
    [<Route("new")>]
    member __.NewGame() =
        let createNewGamePipeline =
            gameManager.CreateNewBoardState Red
            |> persistence.Set cache (GameIdentifier.NewGuid())

        match createNewGamePipeline with
        | Ok id ->
            logger.LogTrace ( sprintf "Created game %s" (id.ToString()) )
            ( id |> converter.GameIdentifierAsViewModel |> __.Ok ) :> IActionResult
        | Error e ->
            logger.LogError e
            __.BadRequest( e ) :> IActionResult

    [<HttpGet>]
    [<Route("{id}")>]
    member __.GetGame (id: GameIdentifier) =
        let getGamePipeline = 
            persistence.Get cache id

        match getGamePipeline with
        | Ok boardState ->
            ( boardState |> converter.BoardStateAsViewModel |> __.Ok ) :> IActionResult
        | Error e -> 
            logger.LogError e;
            __.BadRequest( e ) :> IActionResult
            