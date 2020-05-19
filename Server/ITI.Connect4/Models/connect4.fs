namespace ITI.Connect4
type Player =
    | Red
    | Yellow
type BoardCell = Player option
type BoardColumn = BoardCell array
type Board = BoardColumn array
type BoardState = { Board: Board; Turn: Player }
type GameState = 
    | Running
    | Draw
    | Win of Player