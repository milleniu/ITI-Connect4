namespace ITI.Connect4.Services

open System
open Microsoft.Extensions.Caching.Memory
open ITI.Connect4.Models

[<Sealed>]
type PersistenceService ( memoryCache: IMemoryCache ) =
    let getKey (GameIdentifier id) = id.ToString("N")

    interface IPersistenceService with
        member __.Get (gameIdentifier: GameIdentifier) : Result<BoardState, string> =
            match gameIdentifier |> getKey |> memoryCache.TryGetValue with
            | true, boardState -> Ok ( boardState :?> BoardState )
            | _ -> Error ( sprintf "Game %s does not exists" (id.ToString() ) )

        member __.Exist (gameIdentifier: GameIdentifier) : bool =
            match gameIdentifier |> getKey |> memoryCache.TryGetValue with
             | true, _ -> true
             | _ -> false

        member __.Set (gameIdentifier: GameIdentifier) (data: BoardState): Result<GameIdentifier, string> =
            let id = gameIdentifier |> getKey
            memoryCache.Set( id, data, TimeSpan.FromDays (float 1) ) |> ignore
            Ok gameIdentifier
        