namespace ITI.Connect4.Services

open ITI.Connect4.Models

type ViewModelConverterDependency = {
    PlayerAsViewModel : Player -> PlayerViewModel
    BoardStateAsViewModel : BoardState -> BoardStateViewModel
    GameIdentifierAsViewModel : GameIdentifier -> GameIdentifierViewModel
    GameStateAsViewModel : GameState -> GameStateViewModel

    PlayerFromViewModel : PlayerViewModel -> Result<Player, string>
}

module ViewModelConverter =
    let playerAsViewModel (player: Player) : PlayerViewModel =
        match player with
        | Yellow -> "Yellow"
        | Red -> "Red"

    let boardStateAsViewModel ({ Board = board; Turn = turn } : BoardState) : BoardStateViewModel =
        { Board =
            board
            |> Seq.map (fun column ->
                column
                |> Seq.map (fun cell -> 
                    match cell with
                    | None -> "Empty"
                    | Some player -> player |> playerAsViewModel)
                |> Seq.rev
                |> Seq.toArray)
            |> Seq.toArray
          Turn =
            turn |> playerAsViewModel }

    let gameIdentifierAsViewModel (identifier: GameIdentifier) : GameIdentifierViewModel =
        { Id = (identifier.ToString()) }

    let gameStateAsViewModel (gameState: GameState) : GameStateViewModel =
        { GameState =
            match gameState with
            | Win player -> sprintf "Winner %s" (match player with Red -> "Red" | Yellow -> "Yellow")
            | Draw -> "Draw"
            | Running -> "Running" }

    let playerFromViewModel (playerViewModel: PlayerViewModel) : Result<Player, string> =
        match playerViewModel with
        | "Yellow" -> Ok Yellow
        | "Red" -> Ok Red
        | _ -> Error (sprintf "Invalid player %s" playerViewModel)
