namespace ITI.Connect4.Services

open ITI.Connect4.Models

type GameManagerDependency = {
    CreateNewBoardState : Player -> BoardState
    PutChip : Board -> Player -> Column -> Result<Board, string>
    EvaluateBoad : Board -> GameState
}

type WinningCondition = WinningCondition of (Column -> Row -> Player option)

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
            let chipAt column' row' =
                if (column', row') = (column, row)
                    then Some player
                    else board.[column'].[row']
            Array.init BoardWidth (fun column' -> Array.init BoardHeight (fun row' -> (chipAt column' row')))

        column
        |> validateColumn 
        |> Result.bind (fun column ->
            match getFirstEmptyCell column with
            | Some row -> Ok (insertAt column row)
            | None -> Error (sprintf "No space left in column %d" column))

    let evaluateBoard (board: Board) : GameState =
        let inline (>=<) a (b,c) = a >= b && a <= c

        let horizontalCheck = WinningCondition(fun column row ->
            if (column >=< (0, BoardWidth - 4)) && (row >=< (0, BoardHeight - 1))
            && ([1..3] |> Seq.forall (fun i -> board.[column].[row] = board.[column+i].[row]))
                then board.[column].[row]
                else None)

        let verticalCheck = WinningCondition(fun column row ->
            if (column >=< (0, BoardWidth - 1)) && (row >=< (0, BoardHeight - 4))
            && ([1..3] |> Seq.forall (fun i -> board.[column].[row] = board.[column].[row + i]))
                then board.[column].[row]
                else None)

        let ascendingDiagonal = WinningCondition(fun column row ->
            if (column >=< (3, BoardWidth - 1)) && (row >=< (0, BoardHeight - 4))
            && ([1..3] |> Seq.forall (fun i -> board.[column].[row] = board.[column - i].[row + i]))
                then board.[column].[row]
                else None)

        let descendingDiagonal = WinningCondition(fun column row ->
            if (column >=< (3, BoardWidth - 1)) && (row >=< (3, BoardHeight - 1))
            && ([1..3] |> Seq.forall (fun i -> board.[column].[row] = board.[column - i].[row - i]))
                then board.[column].[row]
                else None)

        let checkAt column row : Player option =
            let rec checkAt' checks =
                match checks with
                | [] -> None
                | WinningCondition( head ) :: tail -> match head column row with Some _ as winner -> winner | None -> checkAt' tail
            checkAt' [ horizontalCheck ; verticalCheck ; ascendingDiagonal ; descendingDiagonal ]

        seq {
            for col in 0..BoardHeight do
                for row in 0..BoardWidth do
                    let checkResult = checkAt col row
                    if checkResult |> Option.isSome then Win checkResult.Value }
        |> Seq.fold (fun a b ->
            match (a, b) with
            | (Win _ as win), _ -> win
            | _, (Win _ as win) -> win
            | _ -> Running ) Running
            