namespace ITI.Connect4.Services

open ITI.Connect4.Models

type GameManagerDependency = {
    CreateNewBoardState : Player -> BoardState
    PutChip : Board -> Player -> Column -> Result<Board, string>
    EvaluateBoad : Board -> GameState
}

module GameManager =
    [<Literal>]
    let BoardWidth = 7
    
    [<Literal>]
    let BoardHeight = 6

    let createNewBoardState (startingPlayer: Player) : BoardState = 
        let newBoard : Board =
            Array.create BoardWidth ( Array.create BoardHeight None )
        { Board = newBoard; Turn = startingPlayer }

    let putChip (board: Board) (player: Player) (column: Column) : Result<Board, string> =
        let validateColumn column =
            if column >= 0 && column < BoardWidth
                then Ok column
                else Error (sprintf "Invalid column %d" column)

        let getFirstEmptyCell column =
            board.[column] |> Array.tryFindIndex Option.isNone

        let insertAt column row =
            let chipAt c r =
                if (c, r) = (column, row)
                    then Some player
                    else board.[c].[r]
            Array.init BoardWidth (fun c -> Array.init BoardHeight (fun r -> (chipAt c r)))

        column
        |> validateColumn 
        |> Result.bind (fun column ->
            match getFirstEmptyCell column with
            | Some row -> Ok (insertAt column row)
            | None -> Error (sprintf "No space left in column %d" column))

    let evaluateBoard (board: Board) : GameState =
        let winningConditions = [ ]
            //match (i, j)
            //if board.[j].[i] = board.[j].[i+1] && board.[j].[i] = board.[j].[i+2] && board.[j].[i] = board.[j].[i+3]
            //    then match board.[j].[i] with Some p -> Win p | None -> Running
            //    else Running))

        let checkAt col row : Player option =
            None

        seq {
            for col in 0..(BoardHeight - 1) do
                for row in 0..(BoardWidth - 1) do
                    if (checkAt col row |> Option.isSome) then Running (* Win of Player *) }
        |> Seq.fold (fun a b ->
            match (a, b) with
            | (Win _ as win), _ -> win
            | _, (Win _ as win) -> win
            | _ -> Running ) Running
            