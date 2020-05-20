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
        | Yellow -> "yellow"
        | Red -> "red"

    let boardStateAsViewModel ({ Board = board; Turn = turn } : BoardState) : BoardStateViewModel =
        let boardViewModel: BoardViewModel =
            board
            |> Seq.map (fun (column: BoardColumn) ->
                column
                |> Seq.map (fun (cell: BoardCell) -> 
                    match cell with
                    | None -> "Empty"
                    | Some player -> player |> playerAsViewModel)
                |> Seq.toArray)
            |> Seq.toArray

        { Board = boardViewModel; Turn = turn |> playerAsViewModel }

    let gameIdentifierAsViewModel (identifier: GameIdentifier) : GameIdentifierViewModel =
        { Id = (identifier.ToString()) }

    let playerFromViewModel (playerViewModel: PlayerViewModel) : Result<Player, string> =
        match playerViewModel with
        | "yellow" -> Ok Yellow
        | "red" -> Ok Red
        | _ -> Error (sprintf "Invalid player %s" playerViewModel)
