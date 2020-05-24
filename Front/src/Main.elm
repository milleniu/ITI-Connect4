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

createModel : String -> Model
createModel gameID =
  { 
    gameId = gameID,
    board = [ 
      [ Empty, Empty, Empty, Empty, Empty, Empty ],
      [ Empty, Empty, Empty, Empty, Empty, Empty ],
      [ Empty, Empty, Empty, Empty, Empty, Empty ],
      [ Empty, Empty, Empty, Empty, Empty, Empty ],
      [ Empty, Empty, Empty, Empty, Empty, Empty ],
      [ Empty, Empty, Empty, Empty, Empty, Empty ],
      [ Empty, Empty, Empty, Empty, Empty, Empty ]
    ],
    turn = Red,
    error = "",
    gameState = ""
  }

updateTurn : Model -> Model 
updateTurn model = 
  case model.turn of 
    Yellow -> { gameId = model.gameId,  board = model.board, turn = Red, error = model.error, gameState = model.gameState}
    _ -> { gameId = model.gameId,  board = model.board, turn = Yellow, error = model.error, gameState = model.gameState }

humanize : Model -> String
humanize model = 
  if String.contains "Winner" model.gameState then
    model.gameState
  else
    "It' s " ++ caseString model.turn ++ " to play"

addErrorModel: Model -> String -> Model
addErrorModel model err = 
  { gameId = model.gameId,  board = model.board, turn = model.turn , error = err, gameState = model.gameState}


-- HTTP

putPlay : Model -> Int -> Cmd Msg
putPlay model column = 
  Http.post 
  { url = "/api/connect4/" ++  model.gameId ++ "/" ++ caseString model.turn ++ "/" ++ String.fromInt column,
    body = Http.emptyBody,
    expect = Http.expectJson PutPlay putPlayDecoder
  }

putPlayDecoder : Decoder String
putPlayDecoder = 
  field "gameState" string

getGame : Cmd Msg
getGame =
  Http.post
    { url = "/api/connect4/new" -- get game
    , body = Http.emptyBody
    , expect = Http.expectJson GetGame getGameDecoder
    }

getGameDecoder : Decoder String
getGameDecoder = 
  field "id" string

getGrid : Model -> Cmd Msg
getGrid model = 
  Http.get 
  { url = "/api/connect4/" ++  model.gameId ,
    expect = Http.expectJson GetGrid getGridDecoder
  }

getGridDecoder : Decoder (List (List State))
getGridDecoder = 
  field "board" ( Json.Decode.list ( Json.Decode.list stateDecoder) )

stateDecoder : Decoder State
stateDecoder = 
  Json.Decode.string
        |> Json.Decode.andThen (\str ->
           case str of
                "Red" ->
                  Json.Decode.succeed Red
                "Yellow" ->
                  Json.Decode.succeed Yellow
                "Empty" ->
                  Json.Decode.succeed Empty
                err ->
                    Json.Decode.fail <| "Unknown state: " ++ err
        )
  
-- VIEW    

view : Model -> Html Msg
view model =
  div [ ]
    [
      div 
      [class "message"] [text (humanize model)]
    , div 
      [class "error"] [text model.error]
    , button
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