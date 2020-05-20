namespace ITI.Connect4.Services

open System
open Microsoft.Extensions.Caching.Memory
open ITI.Connect4.Models

type GameServiceDependency = {
    NewGame : IMemoryCache -> Guid -> Result<Guid, string>
    GetGame : IMemoryCache -> Guid -> Result<BoardStateViewModel, string>
}

module GameService =
    let newGame (cache: IMemoryCache) (id: Guid) : Result<Guid, string> =
        match cache.TryGetValue id with
        | true, _ -> Error ( sprintf "Game %s already exists" ( id.ToString() ) )
        | _ ->
            let newBoard : Board = Array.create 7 ( Array.create 6 None )
            let newBoardState : BoardState = { Board = newBoard; Turn = Red }
            cache.Set( id, newBoardState, TimeSpan.FromDays (float 1) ) |> ignore
            Ok id
            
    let getGame (cache: IMemoryCache) (id: Guid) : Result<BoardStateViewModel, string> =
        let prettyfyBoardState ({ Board = board; Turn = turn }: BoardState) : BoardStateViewModel =
            let toViewModel (player: Player): TurnViewModel =
                match player with
                | Red -> "Red" 
                | Yellow -> "Yellow"

            let boardViewModel: BoardViewModel =
                board
                |> Seq.map (fun (column: BoardColumn) ->
                    column
                    |> Seq.map (fun (cell: BoardCell) -> 
                        match cell with
                        | None -> "Empty"
                        | Some player -> player |> toViewModel)
                    |> Seq.toArray)
                |> Seq.toArray

            { Board = boardViewModel; Turn = turn |> toViewModel }
        
        match cache.TryGetValue id with
        | true, boardState -> Ok ( (boardState :?> BoardState) |> prettyfyBoardState )
        | _ -> Error ( sprintf "Game %s does not exists" (id.ToString() ) )
