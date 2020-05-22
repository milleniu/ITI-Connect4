namespace ITI.Connect4.Models

open System

type Column = int
type Row = int
type Coordinate = Column * Row

type Player = Red | Yellow
type BoardCell = Player option
type BoardColumn = BoardCell array
type Board = BoardColumn array
type BoardState = { Board: Board; Turn: Player }

type GameIdentifier = GameIdentifier of Guid
type GameState = Running | Draw | Win of Player
type GameWinningCondition = GameWinningCondition of (Column -> Row -> Player option)

[<Sealed>]
module Constants =
    [<Literal>]
    let BoardWidth = 7

    [<Literal>]
    let BoardHeight = 6
