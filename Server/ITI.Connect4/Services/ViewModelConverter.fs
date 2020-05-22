namespace ITI.Connect4.Services

open ITI.Connect4.Models

[<Sealed>]
type ViewModelConverter () =
    interface IViewModelConverter with
        member __.PlayerAsViewModel (player: Player) : PlayerViewModel =
            match player with
            | Yellow -> "Yellow"
            | Red -> "Red"

        member __.BoardStateAsViewModel ({ Board = board; Turn = turn } : BoardState) : BoardStateViewModel =
            { Board =
                board
                |> Seq.map (fun column ->
                    column
                    |> Seq.map (fun cell -> 
                        match cell with
                        | None -> "Empty"
                        | Some player -> player |> (__ :> IViewModelConverter).PlayerAsViewModel)
                    |> Seq.rev
                    |> Seq.toArray)
                |> Seq.toArray
              Turn =
                turn |> (__ :> IViewModelConverter).PlayerAsViewModel }

        member __.GameIdentifierAsViewModel (GameIdentifier gameIdentifier) : GameIdentifierViewModel =
            { Id = (gameIdentifier.ToString("N")) }

        member __.GameStateAsViewModel (gameState: GameState) : GameStateViewModel =
            { GameState =
                match gameState with
                | Win player -> sprintf "Winner %s" (match player with Red -> "Red" | Yellow -> "Yellow")
                | Draw -> "Draw"
                | Running -> "Running" }

        member __.PlayerFromViewModel (playerViewModel: PlayerViewModel) : Result<Player, string> =
            match playerViewModel with
            | "Yellow" -> Ok Yellow
            | "Red" -> Ok Red
            | _ -> Error (sprintf "Invalid player %s" playerViewModel)
