module Main exposing (..)

import Browser
import List
import List.Extra exposing (..)
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (..)
import Http
import Json.Decode exposing (Decoder, field, string)

-- INTERNAL MODULES

-- todo: http request

-- import Styles exposing (..)

main =
  Browser.element
    { init = init
    , update = update
    , subscriptions = subscriptions
    , view = view
    }


-- MODEL

type alias Model = 
  {
    gameId : String,
    board : List (List State),
    turn : State,
    error : String,
    gameState : String
  }

init : () -> (Model, Cmd Msg)
init _ = 
  ( (createModel "-1"), getGame )


-- SUBSCRIPTIONS


subscriptions : Model -> Sub Msg
subscriptions model =
  Sub.none

-- UPDATE

type Msg 
  = AddPawn Int
  | NewGame
  | GetGame (Result Http.Error String)
  | PutPlay (Result Http.Error String)
  | GetGrid (Result Http.Error (List (List State)))

type alias Case =
  { state : State
  , id : Int
  }

type State
  = Empty
  | Yellow
  | Red

caseString : State -> String
caseString state = 
  case state of 
    Empty -> 
      "Empty"
    Yellow -> 
      "Yellow"
    Red -> 
      "Red"

update : Msg -> Model -> (Model, Cmd Msg)
update msg model =
  case msg of
    NewGame -> 
      Debug.log "Click"
      (createModel "", getGame)
    AddPawn column ->
      (model, putPlay model column)
    
    GetGame result -> 
      case result of
        Ok id -> 
          Debug.log id
          ( createModel id, Cmd.none)
        Err _ -> ( addErrorModel model "Game error, restart a new game. (GetGame)" , Cmd.none)
    PutPlay result -> 
      case result of
        Ok gameState -> 
          ( updateGameState model gameState, getGrid model)
        Err _ ->
          ( addErrorModel model "Game error, restart a new game. (PutPlay)", Cmd.none)
    GetGrid result -> 
      case result of
        Ok newGrid -> 
          ( updateGrid model newGrid |> updateTurn, Cmd.none) 
        Err _ -> ( addErrorModel model "Game error, restart a new game. (GetGrid)" , Cmd.none)

 -- HELPER

updateGrid : Model -> List (List State) -> Model
updateGrid model grid =
  { gameId = model.gameId, board = grid, turn = model.turn, error = model.error, gameState = model.gameState}

updateGameState : Model -> String -> Model
updateGameState model gameState = 
  { gameId = model.gameId,  board = model.board, turn = model.turn, error = model.error, gameState = gameState}

-- todo
type CaseState
  = Empty
  | PlayerOne
  | PlayerTwo

caseString : CaseState -> String
caseString caseState = 
  case caseState of 
    Empty -> 
      "Empty"
    PlayerOne -> 
      "PlayerOne"
    PlayerTwo -> 
      "PlayerTwo"

update : Msg -> Model -> Model
update msg model =
  case msg of
    NewGame -> 
      Debug.log("Click")
      (createModel "0")
    AddPawn id ->
      Debug.log(String.fromInt(id))
      (createModel "ATTENTION ADDPAWN PAS NEW GAME")

createModel gameID =
  { gameId = gameID,  board =[ [ Empty, Empty, Empty, Empty, Empty, Empty ],[ Empty, Empty, Empty, Empty, Empty, Empty ],[ Empty, Empty, Empty, Empty, Empty, Empty ],[ Empty, Empty, Empty, Empty, Empty, Empty ],[ Empty, Empty, Empty, Empty, Empty, Empty ],[ Empty, Empty, Empty, Empty, Empty, Empty ],[ Empty, Empty, Empty, Empty, Empty, Empty ]] }
         


-- VIEW   

view : Model -> Html Msg
view model =
  div [ ]
    [ button
        [ class "buttons", onClick NewGame ] [text "New Game" ]
    , div 
        [ class "gridContainer"
        , style "display" "flex"
        ]
        (List.indexedMap (\i elm -> div 
                            [ class "column", onClick (AddPawn i) ]
                            (List.map(\n -> div
                                              [ 
                                                class (caseString n)
                                              ]
                                              [ 
                                                li [] []
                                              ]) elm)
                                          
          ) model.board)
    ]