namespace ITI.Connect4.Services

open ITI.Connect4.Models

[<Sealed>]
type GameManager () =
    interface IGameManager with
        member __.CreateNewBoardState (startingPlayer: Player) : BoardState = 
            { Board = Array.create Constants.BoardWidth ( Array.create Constants.BoardHeight None );
              Turn = startingPlayer }

        member __.PutChip (board: Board) (player: Player) (column: Column) : Result<Board, string> =
            let validateColumn column =
                if column >= 0 && column < Constants.BoardWidth
                    then Ok column
                    else Error (sprintf "Invalid column %d" column)

            let getFirstEmptyCell column =
                board.[column] |> Array.tryFindIndex Option.isNone

            let insertAt column row =
                let chipAt column' row' =
                    if (column', row') = (column, row)
                        then Some player
                        else board.[column'].[row']
                Array.init Constants.BoardWidth (fun column' ->
                    Array.init Constants.BoardHeight (fun row' ->
                        (chipAt column' row')))

            column
            |> validateColumn 
            |> Result.bind (fun column ->
                match getFirstEmptyCell column with
                | Some row -> Ok (insertAt column row)
                | None -> Error (sprintf "No space left in column %d" column))

        member __.EvaluateBoard (board: Board) : GameState =
            let inline (>=<) a (b,c) = a >= b && a <= c

            let horizontalCheck = GameWinningCondition(fun column row ->
                if (column >=< (0, Constants.BoardWidth - 4)) && (row >=< (0, Constants.BoardHeight - 1))
                && ([1..3] |> Seq.forall (fun i -> board.[column].[row] = board.[column+i].[row]))
                    then board.[column].[row]
                    else None)

            let verticalCheck = GameWinningCondition(fun column row ->
                if (column >=< (0, Constants.BoardWidth - 1)) && (row >=< (0, Constants.BoardHeight - 4))
                && ([1..3] |> Seq.forall (fun i -> board.[column].[row] = board.[column].[row + i]))
                    then board.[column].[row]
                    else None)

            let ascendingDiagonal = GameWinningCondition(fun column row ->
                if (column >=< (3, Constants.BoardWidth - 1)) && (row >=< (0, Constants.BoardHeight - 4))
                && ([1..3] |> Seq.forall (fun i -> board.[column].[row] = board.[column - i].[row + i]))
                    then board.[column].[row]
                    else None)

            let descendingDiagonal = GameWinningCondition(fun column row ->
                if (column >=< (3, Constants.BoardWidth - 1)) && (row >=< (3, Constants.BoardHeight - 1))
                && ([1..3] |> Seq.forall (fun i -> board.[column].[row] = board.[column - i].[row - i]))
                    then board.[column].[row]
                    else None)

            let checkAt column row : Player option =
                let rec checkAt' checks =
                    match checks with
                    | [] -> None
                    | GameWinningCondition( head ) :: tail ->
                        match head column row with Some _ as winner -> winner | None -> checkAt' tail
                checkAt' [ horizontalCheck ; verticalCheck ; ascendingDiagonal ; descendingDiagonal ]

            seq {
                for col in 0..Constants.BoardHeight do
                    for row in 0..Constants.BoardWidth do
                        let checkResult = checkAt col row
                        if checkResult |> Option.isSome then Win checkResult.Value }
            |> Seq.fold (fun a b -> 
                match (a, b) with
                | (Win _ as win), _ -> win
                | _, (Win _ as win) -> win
                | _ -> Running ) Running
            // TODO: Add Draw Check
            