namespace ITI.Connect4.Services

open System
open ITI.Connect4.Models

type ViewModelConverterDependency = {
    PlayerAsViewModel : Player -> PlayerViewModel
    BoardStateAsViewModel : BoardState -> BoardStateViewModel
    GameIdentifierAsViewModel : GameIdentifier -> GameIdentifierViewModel

    PlayerFromViewModel : PlayerViewModel -> Result<Player, string>
}

module ViewModelConverter =
    let playerAsViewModel (player: Player) : PlayerViewModel =
        match player with
        | Yellow -> "Yellow"
        | Red -> "Red"

    let boardStateAsViewModel ({ Board = board; Turn = turn } : BoardState) : BoardStateViewModel =
        let boardViewModel =
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

        { Board = boardViewModel; Turn = turn |> playerAsViewModel }

    let gameIdentifierAsViewModel (identifier: GameIdentifier) : GameIdentifierViewModel =
        { Id = (identifier.ToString()) }

    let playerFromViewModel (playerViewModel: PlayerViewModel) : Result<Player, string> =
        match playerViewModel with
        | "Yellow" -> Ok Yellow
        | "Red" -> Ok Red
        | _ -> Error (sprintf "Invalid player %s" playerViewModel)
