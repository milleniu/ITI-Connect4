﻿namespace ITI.Connect4.Services

open System
open Microsoft.Extensions.Caching.Memory
open ITI.Connect4.Models

type Connect4ServiceDependency = {
    NewGame : IMemoryCache -> Guid -> Result<Guid, string>
}

module Connect4Service =
    let newGame (cache: IMemoryCache) (id: Guid) : Result<Guid, string> =
        match cache.TryGetValue id with
        | true, _ -> Error ( sprintf "Game %s already exists" ( id.ToString() ) )
        | _ ->
            let newBoard : Board = Array.create 7 ( Array.create 6 None )
            let newBoardState = { Board = newBoard; Turn = Red }
            cache.Set( id, newBoardState, TimeSpan.FromDays (float 1) ) |> ignore
            Ok id