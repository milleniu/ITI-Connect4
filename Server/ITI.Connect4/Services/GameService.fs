namespace ITI.Connect4.Services

open System
open Microsoft.Extensions.Caching.Memory
open ITI.Connect4.Models

type GameServiceDependency = {
    NewGame : IMemoryCache -> Guid -> Result<Guid, string>
    GetGame : IMemoryCache -> Guid -> Result<BoardState, string>
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
            
    let getGame (cache: IMemoryCache) (id: Guid) : Result<BoardState, string> =
        match cache.TryGetValue id with
        | true, boardState -> Ok ( boardState :?> BoardState )
        | _ -> Error ( sprintf "Game %s does not exists" (id.ToString() ) )
