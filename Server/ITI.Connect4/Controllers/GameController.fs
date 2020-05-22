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
            id |> converter.GameIdentifierAsViewModel |> __.Ok :> IActionResult
        | Error e ->
            logger.LogError e
            e |> __.BadRequest :> IActionResult

    [<HttpGet>]
    [<Route("{id}")>]
    member __.GetGame (id: GameIdentifier) =
        let getGamePipeline = 
            persistence.Get cache id

        match getGamePipeline with
        | Ok boardState ->
            boardState |> converter.BoardStateAsViewModel |> __.Ok :> IActionResult
        | Error e -> 
            logger.LogError e;
            e |> __.BadRequest :> IActionResult

    [<HttpPost>]
    [<Route("{id}/{player}/{column}")>]
    member __.PutChip (id: GameIdentifier) (player: string) (column: int) =
        let parametersValidation =
            match (persistence.Get cache id, converter.PlayerFromViewModel player) with
                | Error e, _ -> Error e
                | _, Error e -> Error e
                | Ok boardState, Ok parsedPlayer -> Ok (boardState, parsedPlayer)

        let validatePlayerTurn ((({ BoardState.Board = board; BoardState.Turn = currentTurn })), (playerTurn)) =
            if currentTurn = playerTurn
                then Ok (board, playerTurn)
                else Error (sprintf "It is not %s player turn" player)

        let putChip (board, player) =
            match gameManager.PutChip board player column with
            | Ok board -> Ok (board, player)
            | Error e -> Error e
        
        let persistResult (board, player) = 
            let nextPlayer =
                match player with
                | Yellow -> Red
                | Red -> Yellow
            match persistence.Set cache id ({ BoardState.Board = board; BoardState.Turn = nextPlayer }) with
            | Ok _ -> Ok ({ BoardState.Board = board; BoardState.Turn = nextPlayer })
            | Error e -> Error e

        let evaluateBoard (({ Board = board }): BoardState) =
            gameManager.EvaluateBoad board

        let putChipPipeline =
            parametersValidation
            |> Result.bind validatePlayerTurn
            |> Result.bind putChip
            |> Result.bind persistResult
            |> Result.map evaluateBoard

        match putChipPipeline with
        | Ok gameState ->
            logger.LogTrace (sprintf "Game %s:: Player %s played in column %d" (id.ToString()) player column)
            gameState |> converter.GameStateAsViewModel |> __.Ok :> IActionResult
        | Error e ->
            logger.LogError e
            e |> __.BadRequest :> IActionResult

            