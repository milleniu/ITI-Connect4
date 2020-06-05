namespace ITI.Connect4.Models

type IGameManager =
    abstract member CreateNewBoardState : Player -> BoardState
    abstract member PutChip : Board -> Player -> Column -> Result<Board, string>
    abstract member EvaluateBoard : Board -> GameState

type IPersistenceService =
    abstract member Get : GameIdentifier -> Result<BoardState, string>
    abstract member Exist : GameIdentifier -> bool
    abstract member Set : GameIdentifier -> BoardState -> Result<GameIdentifier, string>

type IViewModelConverter =
    abstract member PlayerAsViewModel : Player -> PlayerViewModel
    abstract member BoardStateAsViewModel : BoardState -> BoardStateViewModel
    abstract member GameIdentifierAsViewModel : GameIdentifier -> GameIdentifierViewModel
    abstract member GameStateAsViewModel : GameState -> GameStateViewModel
    abstract member PlayerFromViewModel : PlayerViewModel -> Result<Player, string>
