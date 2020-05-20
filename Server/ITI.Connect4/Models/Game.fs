namespace ITI.Connect4.Models

open System

type Player =
    | Red
    | Yellow

type BoardCell = Player option
type BoardColumn = BoardCell array
type Board = BoardColumn array
type BoardState = { Board: Board; Turn: Player }

// TODO: Use single case union ?
type GameIdentifier = Guid

type GameState = 
    | Running
    | Draw
    | Win of Player
