namespace ITI.Connect4.Controllers

open System
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open ITI.Connect4.Services

[<ApiController>]
[<Route("api/[controller]")>]
type Connect4Controller ( connect4Service: Connect4ServiceDependency, logger : ILogger<Connect4Controller> ) =
    inherit ControllerBase()

    [<HttpPost>]
    [<Route("new")>]
    member __.NewGame() =
        let { NewGame = newGame } = connect4Service
        match newGame (Guid.NewGuid()) with
        | Ok id -> __.Ok( id ) :> IActionResult
        | Error e -> logger.LogError e; __.BadRequest( e ) :> IActionResult
