namespace ITI.Connect4.Services

open System
open Microsoft.Extensions.Caching.Memory
open ITI.Connect4.Models

type PersistenceServiceDependency = {
    Get : IMemoryCache -> GameIdentifier -> Result<BoardState, string>
    Exist : IMemoryCache -> GameIdentifier -> bool
    Set : IMemoryCache -> GameIdentifier -> BoardState -> Result<GameIdentifier, string>
}

module PersistenceService =
    let get (cache: IMemoryCache) (id: GameIdentifier) : Result<BoardState, string> =
        match cache.TryGetValue id with
        | true, boardState -> Ok ( boardState :?> BoardState )
        | _ -> Error ( sprintf "Game %s does not exists" (id.ToString() ) )

    let exist (cache: IMemoryCache) (id: GameIdentifier) : bool =
        match cache.TryGetValue id with
         | true, _ -> true
         | _ -> false

    let set (cache: IMemoryCache) (id: GameIdentifier) (data: BoardState): Result<GameIdentifier, string> =
        cache.Set( id, data, TimeSpan.FromDays (float 1) ) |> ignore
        Ok id            
        