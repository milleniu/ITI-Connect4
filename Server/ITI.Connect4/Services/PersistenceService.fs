namespace ITI.Connect4.Services

open System
open Microsoft.Extensions.Caching.Memory
open ITI.Connect4.Models

type PersistenceServiceDependency = {
    Get : IMemoryCache -> Guid -> Result<BoardState, string>
    Set : IMemoryCache -> Guid -> BoardState -> Result<Guid, string>
}

module PersistenceService =
    let get (cache: IMemoryCache) (id: Guid) : Result<BoardState, string> =
        match cache.TryGetValue id with
        | true, boardState -> Ok ( boardState :?> BoardState )
        | _ -> Error ( sprintf "Game %s does not exists" (id.ToString() ) )

    let set (cache: IMemoryCache) (id: Guid) (data: BoardState): Result<Guid, string> =
        match cache.TryGetValue id with
        | true, _ -> Error ( sprintf "Game %s already exists" ( id.ToString() ) )
        | _ ->
            cache.Set( id, data, TimeSpan.FromDays (float 1) ) |> ignore
            Ok id
        