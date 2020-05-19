namespace ITI.Connect4.Models

type BoardCellViewModel = string
type BoardColumnViewModel = BoardCellViewModel array
type BoardViewModel = BoardColumnViewModel array
type TurnViewModel = string
type BoardStateViewModel = { Board: BoardViewModel; Turn: TurnViewModel }