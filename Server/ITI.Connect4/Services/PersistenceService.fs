namespace ITI.Connect4.Services

open System
open Microsoft.Extensions.Caching.Memory
open ITI.Connect4.Models

type PersistenceServiceDependency = {
    Get : IMemoryCache -> GameIdentifier -> Result<BoardState, string>
    Set : IMemoryCache -> GameIdentifier -> BoardState -> Result<GameIdentifier, string>
}

module PersistenceService =
    let get (cache: IMemoryCache) (id: GameIdentifier) : Result<BoardState, string> =
        match cache.TryGetValue id with
        | true, boardState -> Ok ( boardState :?> BoardState )
        | _ -> Error ( sprintf "Game %s does not exists" (id.ToString() ) )

    let set (cache: IMemoryCache) (id: GameIdentifier) (data: BoardState): Result<GameIdentifier, string> =
        match cache.TryGetValue id with
        | true, _ -> Error ( sprintf "Game %s already exists" ( id.ToString() ) )
        | _ ->
            cache.Set( id, data, TimeSpan.FromDays (float 1) ) |> ignore
            Ok id
        