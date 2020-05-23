namespace ITI.Connect4.Controllers

open System
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open ITI.Connect4.Models

[<ApiController>]
[<Route("api/connect4")>]
type Connect4Controller ( logger : ILogger<Connect4Controller>,
    gameManager : IGameManager,
    persistenceService: IPersistenceService,
    viewModelConverter : IViewModelConverter ) =

    inherit ControllerBase()

    [<HttpPost>]
    [<Route("new")>]
    member __.NewGame() =
        let createNewGamePipeline =
            gameManager.CreateNewBoardState Red
            |> persistenceService.Set (GameIdentifier(Guid.NewGuid()))

        match createNewGamePipeline with
        | Ok id ->
            logger.LogTrace ( sprintf "Created game %s" (id.ToString()) )
            id |> viewModelConverter.GameIdentifierAsViewModel |> __.Ok :> IActionResult
        | Error e ->
            logger.LogError e
            e |> __.BadRequest :> IActionResult

    [<HttpGet>]
    [<Route("{id}")>]
    member __.GetGame (id: Guid) =
        match persistenceService.Get (GameIdentifier(id)) with
        | Ok boardState ->
            boardState |> viewModelConverter.BoardStateAsViewModel |> __.Ok :> IActionResult
        | Error e -> 
            logger.LogError e;
            e |> __.BadRequest :> IActionResult

    [<HttpPost>]
    [<Route("{id}/{player}/{column}")>]
    member __.PutChip (id: Guid) (player: string) (column: int) =
        let parametersValidation =
            match (persistenceService.Get (GameIdentifier(id)), viewModelConverter.PlayerFromViewModel player) with
            | Error e, _ -> Error e
            | _, Error e -> Error e
            | Ok boardState, Ok parsedPlayer -> Ok (boardState, parsedPlayer)

        let validatePlayerTurn ((({ BoardState.Board = board; BoardState.Turn = currentTurn })), (playerTurn)) =
            match currentTurn with
            | Some turn when turn = playerTurn -> Ok (board, playerTurn)
            | _ -> Error (sprintf "It is not %s player turn" player)

        let putChip (board, player) =
            match gameManager.PutChip board player column with
            | Ok board -> Ok (board, player)
            | Error e -> Error e
        
        let evaluateBoard (board, player) =
            (board, player, gameManager.EvaluateBoard board)

        let persistResult (board, player, gameState) = 
            let nextPlayer =
                match gameState with
                | Draw-> None
                | Win _ -> None
                | _ -> Some (match player with Yellow -> Red | Red -> Yellow)
            let data = { BoardState.Board = board; BoardState.Turn = nextPlayer }
            match persistenceService.Set (GameIdentifier(id)) data with
            | Ok _ -> Ok (board, player, gameState)
            | Error e -> Error e

        let putChipPipeline =
            parametersValidation
            |> Result.bind validatePlayerTurn
            |> Result.bind putChip
            |> Result.map evaluateBoard
            |> Result.bind persistResult

        match putChipPipeline with
        | Ok (_, _, gameState) ->
            logger.LogTrace (sprintf "Game %s:: Player %s played in column %d" (id.ToString()) player column)
            gameState |> viewModelConverter.GameStateAsViewModel |> __.Ok :> IActionResult
        | Error e ->
            logger.LogError e
            e |> __.BadRequest :> IActionResult

            