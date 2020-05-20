namespace ITI.Connect4.Services

open ITI.Connect4.Models

type GameManagerDependency = {
    CreateNewBoardState : Player -> BoardState
}

module GameManager =
    let createNewBoardState (startingPlayer: Player) : BoardState = 
        let newBoard : Board = Array.create 7 ( Array.create 6 None )
        { Board = newBoard; Turn = startingPlayer }