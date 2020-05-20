namespace ITI.Connect4.Models

type BoardCellViewModel = string
type BoardColumnViewModel = BoardCellViewModel array
type BoardViewModel = BoardColumnViewModel array
type PlayerViewModel = string
type BoardStateViewModel = { Board: BoardViewModel; Turn: PlayerViewModel }
type GameIdentifierViewModel = { Id: string }